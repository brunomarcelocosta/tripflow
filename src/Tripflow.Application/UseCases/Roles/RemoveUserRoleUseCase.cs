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

public sealed class RemoveUserRoleUseCase(
    ITenantContext tenantContext,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    IUserProfileRepository userProfileRepository,
    IRoleRepository roleRepository,
    IKeycloakRoleService keycloakRoleService,
    IOptions<KeycloakOptions> keycloakOptions,
    IValidator<RemoveUserRoleValidationRequest> validator,
    ILogger<RemoveUserRoleUseCase> logger) : IRemoveUserRoleUseCase
{
    public string ClassName = nameof(RemoveUserRoleUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<bool>> ExecuteAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<bool>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Failure("Tenant não resolvido para o usuário atual.");

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.UsersManage,
                cancellationToken))
        {
            return Result<bool>.Forbidden();
        }

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        var validationRequest = new RemoveUserRoleValidationRequest(userId, roleId);

        var validationResult = await validator.ValidateAsync(validationRequest, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                "{ClassName} | {Method} | Erro de validação | UserId={UserId} | RoleId={RoleId}",
                ClassName,
                Method,
                userId,
                roleId);

            var firstError = validationResult.Errors.FirstOrDefault();

            return Result<bool>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        var tenantId = tenantContext.TenantId.Value;

        await userProfileRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var profile = await userProfileRepository.GetTrackedByIdInTenantAsync(tenantId, userId, cancellationToken);

            profile!.RemoveRole(roleId);

            await userProfileRepository.UpdateAsync(profile, cancellationToken);

            var roleIds = profile.UserRoles.Select(x => x.RoleId).ToList();
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
                "{ClassName} | {Method} | Role removida com sucesso | UserId={UserId} | RoleId={RoleId}",
                ClassName,
                Method,
                userId,
                roleId);

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "{ClassName} | {Method} | Erro inesperado ao remover role | UpdatedBy={UpdatedBy} | {Message}",
                ClassName,
                Method,
                updatedBy,
                ex.Message);

            await userProfileRepository.RollbackTransactionAsync(cancellationToken);

            return Result<bool>.Failure("Erro inesperado ao remover role do usuário.");
        }
    }
}
