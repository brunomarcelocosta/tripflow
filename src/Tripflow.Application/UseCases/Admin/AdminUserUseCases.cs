using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Admin;
using Tripflow.Application.Helpers;
using Tripflow.Application.Services.Audit;
using Tripflow.Application.UseCases.Admin.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Admin;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Integrations.Keycloak.Interfaces;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Enums;
using FluentValidation;

namespace Tripflow.Application.UseCases.Admin;

public sealed class GetAdminTenantUsersUseCase(
    IUserProfileRepository userProfileRepository,
    ITenantRepository tenantRepository,
    IUserContext userContext) : IGetAdminTenantUsersUseCase
{
    public async Task<Result<PagedResponse<AdminUserResponse>>> ExecuteAsync(
        Guid tenantId,
        AdminUserFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<AdminUserResponse>>.Forbidden();

        var tenant = await tenantRepository.GetByIdAsync(tenantId);

        if (tenant is null)
            return Result<PagedResponse<AdminUserResponse>>.Failure("Tenant não encontrado.");

        var filter = request.ToExpression();
        var orderBy = AdminOrderByHelper.BuildUser(request.SortBy);

        var paged = await userProfileRepository.GetPagedByTenantForAdminAsync(
            tenantId,
            filter,
            orderBy,
            request.SortDesc,
            request.Page,
            request.PageSize,
            cancellationToken);

        var mapped = paged.Items.Select(AdminMappingHelper.MapUser).ToList();

        return Result<PagedResponse<AdminUserResponse>>.Ok(new PagedResponse<AdminUserResponse>(
            mapped,
            paged.Page,
            paged.PageSize,
            paged.TotalItems,
            paged.TotalPages));
    }
}

public sealed class GetAdminUsersUseCase(
    IUserProfileRepository userProfileRepository,
    IUserContext userContext) : IGetAdminUsersUseCase
{
    public async Task<Result<PagedResponse<AdminUserResponse>>> ExecuteAsync(
        AdminUserFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<AdminUserResponse>>.Forbidden();

        var filter = request.ToExpression();
        var orderBy = AdminOrderByHelper.BuildUser(request.SortBy);

        var paged = await userProfileRepository.GetPagedForAdminAsync(
            request.TenantId,
            filter,
            orderBy,
            request.SortDesc,
            request.Page,
            request.PageSize,
            cancellationToken);

        var mapped = paged.Items.Select(AdminMappingHelper.MapUser).ToList();

        return Result<PagedResponse<AdminUserResponse>>.Ok(new PagedResponse<AdminUserResponse>(
            mapped,
            paged.Page,
            paged.PageSize,
            paged.TotalItems,
            paged.TotalPages));
    }
}

public sealed class GetAdminUserByIdUseCase(
    IUserProfileRepository userProfileRepository,
    IPlatformAdminRepository platformAdminRepository,
    IUserContext userContext) : IGetAdminUserByIdUseCase
{
    public async Task<Result<AdminUserDetailResponse?>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<AdminUserDetailResponse?>.Forbidden();

        var user = await userProfileRepository.GetByIdForAdminAsync(userId, cancellationToken);

        if (user is null)
            return Result<AdminUserDetailResponse?>.Ok(null);

        var recentAuditLogs = await platformAdminRepository.GetRecentAuditLogsByUserAsync(userId, 10, cancellationToken);
        var recentQuotes = await platformAdminRepository.GetRecentQuotesByUserAsync(userId, 5, cancellationToken);
        var recentProposals = await platformAdminRepository.GetRecentProposalsByTenantAsync(user.TenantId, 5, cancellationToken);

        var baseResponse = AdminMappingHelper.MapUser(user);

        var response = new AdminUserDetailResponse
        {
            Id = baseResponse.Id,
            TenantId = baseResponse.TenantId,
            TenantTradeName = baseResponse.TenantTradeName,
            IdentityProviderUserId = baseResponse.IdentityProviderUserId,
            FullName = baseResponse.FullName,
            Email = baseResponse.Email,
            Phone = baseResponse.Phone,
            Status = baseResponse.Status,
            Roles = baseResponse.Roles,
            CreatedAtUtc = baseResponse.CreatedAtUtc,
            UpdatedAtUtc = baseResponse.UpdatedAtUtc,
            Permissions = AdminMappingHelper.MapPermissions(user),
            RecentAuditLogs = recentAuditLogs.Select(AdminMappingHelper.MapAuditLogSummary).ToList(),
            RecentQuotesCreated = recentQuotes.Select(AdminMappingHelper.MapRecentQuote).ToList(),
            RecentProposalsGenerated = recentProposals.Select(AdminMappingHelper.MapRecentProposal).ToList()
        };

        return Result<AdminUserDetailResponse?>.Ok(response);
    }
}

public sealed class UpdateAdminUserUseCase(
    IUserProfileRepository userProfileRepository,
    IKeycloakUserService keycloakUserService,
    IAuditService auditService,
    IValidator<UpdateAdminUserRequest> validator,
    IUserContext userContext) : IUpdateAdminUserUseCase
{
    public async Task<Result<AdminUserResponse?>> ExecuteAsync(
        Guid userId,
        UpdateAdminUserRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<AdminUserResponse?>.Forbidden();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault();
            return Result<AdminUserResponse?>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        if (request.Status is UserStatus.Blocked or UserStatus.Removed)
        {
            var guard = await AdminUserManagementGuard.ValidateAsync(
                userId,
                userProfileRepository,
                userContext,
                cancellationToken);

            if (guard is not null)
                return guard;
        }

        var user = await userProfileRepository.GetTrackedByIdForAdminAsync(userId, cancellationToken);

        if (user is null)
            return Result<AdminUserResponse?>.Ok(null);

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailExists = await userProfileRepository.ExistsByEmailAsync(request.Email, cancellationToken);

            if (emailExists)
                return Result<AdminUserResponse?>.Failure("Já existe outro usuário cadastrado com este e-mail.");
        }

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        user.UpdateProfile(request.FullName, request.Email, request.Phone, request.Status, updatedBy);
        await userProfileRepository.UpdateAsync(user, cancellationToken);

        if (!string.IsNullOrWhiteSpace(user.IdentityProviderUserId))
        {
            var enabled = request.Status is UserStatus.Active or UserStatus.Invited;
            var keycloakUpdated = await keycloakUserService.UpdateUserAsync(
                user.IdentityProviderUserId,
                request.Email,
                request.FullName,
                enabled,
                cancellationToken);

            if (!keycloakUpdated)
                return Result<AdminUserResponse?>.Failure("Não foi possível atualizar o usuário no Keycloak.");
        }

        await auditService.RegisterAsync(new AuditLogRequest(
            user.TenantId,
            userId,
            AuditActions.UserUpdated,
            nameof(UserProfile),
            userId,
            $"Usuário {user.Email} atualizado."), cancellationToken);

        var refreshed = await userProfileRepository.GetByIdForAdminAsync(userId, cancellationToken);
        return Result<AdminUserResponse?>.Ok(refreshed is null ? null : AdminMappingHelper.MapUser(refreshed));
    }
}

public sealed class SetAdminUserPasswordUseCase(
    IUserProfileRepository userProfileRepository,
    IKeycloakUserService keycloakUserService,
    IAuditService auditService,
    IValidator<SetAdminUserPasswordRequest> validator,
    IUserContext userContext) : ISetAdminUserPasswordUseCase
{
    public async Task<Result<bool>> ExecuteAsync(
        Guid userId,
        SetAdminUserPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<bool>.Forbidden();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault();
            return Result<bool>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        var user = await userProfileRepository.GetByIdForAdminAsync(userId, cancellationToken);

        if (user is null)
            return Result<bool>.Ok(false);

        if (string.IsNullOrWhiteSpace(user.IdentityProviderUserId))
            return Result<bool>.Failure("Usuário não possui vínculo com o Keycloak.");

        var passwordSet = await keycloakUserService.SetPasswordAsync(
            user.IdentityProviderUserId,
            request.Password,
            request.Temporary,
            cancellationToken);

        if (!passwordSet)
            return Result<bool>.Failure("Não foi possível definir a senha no Keycloak.");

        await auditService.RegisterAsync(new AuditLogRequest(
            user.TenantId,
            userId,
            AuditActions.UserPasswordChanged,
            nameof(UserProfile),
            userId,
            $"Senha do usuário {user.Email} alterada pelo administrador."), cancellationToken);

        return Result<bool>.Ok(true);
    }
}

public sealed class ActivateAdminUserUseCase(
    IUserProfileRepository userProfileRepository,
    IKeycloakUserService keycloakUserService,
    IAuditService auditService,
    IUserContext userContext) : IActivateAdminUserUseCase
{
    public async Task<Result<AdminUserResponse?>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<AdminUserResponse?>.Forbidden();

        var user = await userProfileRepository.GetTrackedByIdForAdminAsync(userId, cancellationToken);

        if (user is null)
            return Result<AdminUserResponse?>.Ok(null);

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        user.Activate(updatedBy);
        await userProfileRepository.UpdateAsync(user, cancellationToken);

        if (!string.IsNullOrWhiteSpace(user.IdentityProviderUserId))
            await keycloakUserService.EnableUserAsync(user.IdentityProviderUserId, cancellationToken);

        await auditService.RegisterAsync(new AuditLogRequest(
            user.TenantId,
            userId,
            AuditActions.UserActivated,
            nameof(UserProfile),
            userId,
            $"Usuário {user.Email} ativado."), cancellationToken);

        var refreshed = await userProfileRepository.GetByIdForAdminAsync(userId, cancellationToken);
        return Result<AdminUserResponse?>.Ok(refreshed is null ? null : AdminMappingHelper.MapUser(refreshed));
    }
}

public sealed class BlockAdminUserUseCase(
    IUserProfileRepository userProfileRepository,
    IKeycloakUserService keycloakUserService,
    IAuditService auditService,
    IUserContext userContext) : IBlockAdminUserUseCase
{
    public async Task<Result<AdminUserResponse?>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<AdminUserResponse?>.Forbidden();

        var guard = await AdminUserManagementGuard.ValidateAsync(userId, userProfileRepository, userContext, cancellationToken);

        if (guard is not null)
            return guard;

        var user = await userProfileRepository.GetTrackedByIdForAdminAsync(userId, cancellationToken);

        if (user is null)
            return Result<AdminUserResponse?>.Ok(null);

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        user.Block(updatedBy);
        await userProfileRepository.UpdateAsync(user, cancellationToken);

        if (!string.IsNullOrWhiteSpace(user.IdentityProviderUserId))
            await keycloakUserService.DisableUserAsync(user.IdentityProviderUserId, cancellationToken);

        await auditService.RegisterAsync(new AuditLogRequest(
            user.TenantId,
            userId,
            AuditActions.UserBlocked,
            nameof(UserProfile),
            userId,
            $"Usuário {user.Email} bloqueado."), cancellationToken);

        var refreshed = await userProfileRepository.GetByIdForAdminAsync(userId, cancellationToken);
        return Result<AdminUserResponse?>.Ok(refreshed is null ? null : AdminMappingHelper.MapUser(refreshed));
    }
}

public sealed class RemoveAdminUserUseCase(
    IUserProfileRepository userProfileRepository,
    IKeycloakUserService keycloakUserService,
    IAuditService auditService,
    IUserContext userContext) : IRemoveAdminUserUseCase
{
    public async Task<Result<AdminUserResponse?>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<AdminUserResponse?>.Forbidden();

        var guard = await AdminUserManagementGuard.ValidateAsync(userId, userProfileRepository, userContext, cancellationToken);

        if (guard is not null)
            return guard;

        var user = await userProfileRepository.GetTrackedByIdForAdminAsync(userId, cancellationToken);

        if (user is null)
            return Result<AdminUserResponse?>.Ok(null);

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        user.Remove(updatedBy);
        await userProfileRepository.UpdateAsync(user, cancellationToken);

        if (!string.IsNullOrWhiteSpace(user.IdentityProviderUserId))
            await keycloakUserService.DisableUserAsync(user.IdentityProviderUserId, cancellationToken);

        await auditService.RegisterAsync(new AuditLogRequest(
            user.TenantId,
            userId,
            AuditActions.UserRemoved,
            nameof(UserProfile),
            userId,
            $"Usuário {user.Email} removido."), cancellationToken);

        var refreshed = await userProfileRepository.GetByIdForAdminAsync(userId, cancellationToken);
        return Result<AdminUserResponse?>.Ok(refreshed is null ? null : AdminMappingHelper.MapUser(refreshed));
    }
}

internal static class AdminUserManagementGuard
{
    public static async Task<Result<AdminUserResponse?>?> ValidateAsync(
        Guid userId,
        IUserProfileRepository userProfileRepository,
        IUserContext userContext,
        CancellationToken cancellationToken)
    {
        var target = await userProfileRepository.GetByIdForAdminAsync(userId, cancellationToken);

        if (target is null)
            return null;

        if (!string.IsNullOrWhiteSpace(userContext.IdentityProviderUserId) &&
            string.Equals(target.IdentityProviderUserId, userContext.IdentityProviderUserId, StringComparison.OrdinalIgnoreCase))
        {
            return Result<AdminUserResponse?>.Failure("Não é permitido bloquear ou remover o próprio usuário.");
        }

        if (await userProfileRepository.IsPlatformAdminAsync(userId, cancellationToken))
        {
            var activeAdmins = await userProfileRepository.CountActivePlatformAdminsAsync(cancellationToken);

            if (activeAdmins <= 1)
                return Result<AdminUserResponse?>.Failure("Não é permitido alterar o último administrador ativo da plataforma.");
        }

        return null;
    }
}
