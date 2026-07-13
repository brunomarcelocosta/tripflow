using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Roles;
using Tripflow.Application.UseCases.Roles.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;
using Tripflow.Infra.Integrations.Keycloak;
using Tripflow.Infra.Integrations.Keycloak.Interfaces;

namespace Tripflow.Application.UseCases.Roles;

public sealed class AssignUserRolesUseCase(
    ITenantContext tenantContext,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    IUserProfileRepository userProfileRepository,
    IRoleRepository roleRepository,
    IKeycloakRoleService keycloakRoleService,
    IOptions<KeycloakOptions> keycloakOptions,
    IValidator<AssignUserRolesValidationRequest> validator,
    IMapper mapper,
    ILogger<AssignUserRolesUseCase> logger) : IAssignUserRolesUseCase
{
    public string ClassName = nameof(AssignUserRolesUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<IReadOnlyList<string>>> ExecuteAsync(
        Guid userId,
        AssignUserRolesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<IReadOnlyList<string>>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<IReadOnlyList<string>>.Failure("Tenant não resolvido para o usuário atual.");

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.UsersManage,
                cancellationToken))
        {
            return Result<IReadOnlyList<string>>.Forbidden();
        }

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        var validationRequest = AssignUserRolesValidationRequest.From(userId, request);

        var validationResult = await validator.ValidateAsync(validationRequest, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("{ClassName} | {Method} | Erro de validação | UserId={UserId}", ClassName, Method, userId);

            var firstError = validationResult.Errors.FirstOrDefault();

            return Result<IReadOnlyList<string>>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        var tenantId = tenantContext.TenantId.Value;
        var roles = await ResolveRolesAsync(tenantId, request, cancellationToken);

        if (roles.Count != (request.RoleIds?.Count ?? request.RoleNames?.Count ?? 0))
            return Result<IReadOnlyList<string>>.Failure("Uma ou mais roles informadas não pertencem a esta empresa.");

        await userProfileRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var profile = await userProfileRepository.GetTrackedByIdInTenantAsync(tenantId, userId, cancellationToken);

            foreach (var role in roles)
                profile!.AddRole(role.Id);

            await userProfileRepository.UpdateAsync(profile!, cancellationToken);

            var roleIds = profile!.UserRoles.Select(x => x.RoleId).ToList();
            var allRoles = await roleRepository.GetByIdsAsync(tenantId, roleIds, cancellationToken);

            var managedPrefix = keycloakOptions.Value.Admin.ManagedRolePrefix;
            var keycloakRoles = allRoles
                .Select(x => $"{managedPrefix}{x.Name}")
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            await keycloakRoleService.SyncRealmRolesAsync(
                profile.IdentityProviderUserId,
                keycloakRoles,
                cancellationToken);

            await userProfileRepository.CommitTransactionAsync(cancellationToken);

            logger.LogInformation(
                "{ClassName} | {Method} | Roles atribuídas com sucesso | UserId={UserId}",
                ClassName,
                Method,
                userId);

            var assigned = mapper.Map<IReadOnlyList<string>>(allRoles);

            return Result<IReadOnlyList<string>>.Ok(assigned);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "{ClassName} | {Method} | Erro inesperado ao atribuir roles | UpdatedBy={UpdatedBy} | {Message}",
                ClassName,
                Method,
                updatedBy,
                ex.Message);

            await userProfileRepository.RollbackTransactionAsync(cancellationToken);

            return Result<IReadOnlyList<string>>.Failure("Erro inesperado ao atribuir roles ao usuário.");
        }
    }

    private async Task<List<Domain.Entities.Identity.Role>> ResolveRolesAsync(
        Guid tenantId,
        AssignUserRolesRequest request,
        CancellationToken cancellationToken)
    {
        if (request.RoleIds is { Count: > 0 })
            return await roleRepository.GetByIdsAsync(tenantId, request.RoleIds, cancellationToken);

        if (request.RoleNames is { Count: > 0 })
            return await roleRepository.GetByNamesAsync(tenantId, request.RoleNames, cancellationToken);

        return [];
    }
}
