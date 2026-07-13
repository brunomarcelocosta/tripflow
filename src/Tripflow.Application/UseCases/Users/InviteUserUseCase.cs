using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Users;
using Tripflow.Application.DTOs.Responses.Users;
using Tripflow.Application.Services.Subscriptions;
using Tripflow.Application.UseCases.Users.Interfaces;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;
using Tripflow.Infra.Integrations.Keycloak;
using Tripflow.Infra.Integrations.Keycloak.Interfaces;

namespace Tripflow.Application.UseCases.Users;

public sealed class InviteUserUseCase(
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IUserProfileRepository userProfileRepository,
    IRoleRepository roleRepository,
    IUserInvitationRepository userInvitationRepository,
    IKeycloakUserService keycloakUserService,
    IKeycloakRoleService keycloakRoleService,
    IOptions<KeycloakOptions> keycloakOptions,
    IValidator<InviteUserRequest> validator,
    ITenantUsageService tenantUsageService,
    IMapper mapper,
    ILogger<InviteUserUseCase> logger) : IInviteUserUseCase
{
    public string ClassName = nameof(InviteUserUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<InviteUserResponse>> ExecuteAsync(
        InviteUserRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<InviteUserResponse>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<InviteUserResponse>.Failure("Tenant não resolvido para o usuário atual.");

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.UsersManage,
                cancellationToken))
        {
            return Result<InviteUserResponse>.Forbidden();
        }

        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("{ClassName} | {Method} | Erro de validação | UserId={UserId}", ClassName, Method, createdBy);

            var firstError = validationResult.Errors.FirstOrDefault();

            return Result<InviteUserResponse>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var tenantId = tenantContext.TenantId.Value;

        var roles = await ResolveRolesAsync(tenantId, request, cancellationToken);

        if (roles.Count != (request.RoleIds?.Count ?? request.RoleNames?.Count ?? 0))
            return Result<InviteUserResponse>.Failure("Uma ou mais roles informadas não pertencem a esta empresa.");

        var hasUsersLimit = await tenantUsageService.HasAvailableLimitAsync(ITenantUsageService.Users, cancellationToken);
        if (!hasUsersLimit)
            return Result<InviteUserResponse>.Failure("Limite de usuários do plano atingido.");

        var keycloakUserId = await keycloakUserService.CreateUserAsync(
            email,
            request.FullName.Trim(),
            enabled: true,
            emailVerified: false,
            cancellationToken);

        if (string.IsNullOrWhiteSpace(keycloakUserId))
            return Result<InviteUserResponse>.Failure("Não foi possível criar o usuário no Keycloak.");

        await userProfileRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var profile = new UserProfile(
                tenantId,
                keycloakUserId,
                request.FullName.Trim(),
                email,
                request.Phone?.Trim(),
                UserStatus.Invited,
                createdBy);

            foreach (var role in roles)
                profile.AddRole(role.Id);

            await userProfileRepository.AddAsync(profile, cancellationToken);

            var managedPrefix = keycloakOptions.Value.Admin.ManagedRolePrefix;
            var keycloakRoles = roles
                .Select(x => $"{managedPrefix}{x.Name}")
                .ToList();

            await keycloakRoleService.SyncRealmRolesAsync(keycloakUserId, keycloakRoles, cancellationToken);

            var inviteEmailSent = false;

            if (request.SendInviteEmail)
            {
                try
                {
                    inviteEmailSent = await keycloakUserService.SendResetPasswordEmailAsync(
                        keycloakUserId,
                        cancellationToken);

                    if (!inviteEmailSent)
                    {
                        logger.LogWarning(
                            "{ClassName} | {Method} | Convite criado, mas o Keycloak não enviou e-mail | Email={Email}",
                            ClassName,
                            Method,
                            email);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(
                        ex,
                        "{ClassName} | {Method} | Convite criado, mas falhou o envio de e-mail | Email={Email}",
                        ClassName,
                        Method,
                        email);
                }
            }

            var tokenHash = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes($"{profile.Id}:{email}:{Guid.NewGuid()}")));

            var invitation = new UserInvitation(
                tenantId,
                email,
                request.FullName.Trim(),
                tokenHash,
                DateTime.UtcNow.AddDays(7),
                createdBy);

            await userInvitationRepository.AddAsync(invitation, cancellationToken);

            var incremented = await tenantUsageService.IncrementAsync(ITenantUsageService.Users, cancellationToken: cancellationToken);
            if (!incremented)
            {
                await userProfileRepository.RollbackTransactionAsync(cancellationToken);
                return Result<InviteUserResponse>.Failure("Não foi possível atualizar o consumo de usuários.");
            }

            await userProfileRepository.CommitTransactionAsync(cancellationToken);

            logger.LogInformation(
                "{ClassName} | {Method} | Usuário convidado com sucesso | UserProfileId={UserProfileId}",
                ClassName,
                Method,
                profile.Id);

            var response = mapper.Map<InviteUserResponse>(
                profile,
                opt =>
                {
                    opt.Items["RoleNames"] = roles.Select(x => x.Name).ToArray();
                    opt.Items["InviteEmailSent"] = inviteEmailSent;
                });

            return Result<InviteUserResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "{ClassName} | {Method} | Erro inesperado ao convidar usuário | UserId={UserId} | {Message}",
                ClassName,
                Method,
                createdBy,
                ex.Message);

            await userProfileRepository.RollbackTransactionAsync(cancellationToken);

            return Result<InviteUserResponse>.Failure("Erro inesperado ao convidar usuário.");
        }
    }

    private async Task<List<Role>> ResolveRolesAsync(
        Guid tenantId,
        InviteUserRequest request,
        CancellationToken cancellationToken)
    {
        if (request.RoleIds is { Count: > 0 })
            return await roleRepository.GetByIdsAsync(tenantId, request.RoleIds, cancellationToken);

        if (request.RoleNames is { Count: > 0 })
            return await roleRepository.GetByNamesAsync(tenantId, request.RoleNames, cancellationToken);

        return [];
    }
}
