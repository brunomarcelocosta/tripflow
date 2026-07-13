using FluentValidation;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Admin;
using Tripflow.Application.Helpers;
using Tripflow.Application.Services.Audit;
using Tripflow.Application.UseCases.Admin.Interfaces;
using Tripflow.Domain.Entities.Admin;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Admin;
using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.Application.UseCases.Admin;

public sealed class CreateSupportSessionUseCase(
    ISupportSessionRepository supportSessionRepository,
    IUserProfileRepository userProfileRepository,
    ITenantRepository tenantRepository,
    IAuditService auditService,
    IUserContext userContext,
    IValidator<CreateSupportSessionRequest> validator) : ICreateSupportSessionUseCase
{
    public async Task<Result<SupportSessionResponse>> ExecuteAsync(
        CreateSupportSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<SupportSessionResponse>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return Result<SupportSessionResponse>.Failure(validation.Errors.First().ErrorMessage);

        var admin = await userProfileRepository.GetByIdentityProviderUserIdAsync(
            userContext.IdentityProviderUserId!,
            cancellationToken);

        if (admin is null)
            return Result<SupportSessionResponse>.Failure("Perfil administrativo não encontrado.");

        var tenant = await tenantRepository.GetByIdAsync(request.TenantId);

        if (tenant is null)
            return Result<SupportSessionResponse>.Failure("Tenant não encontrado.");

        var activeSession = await supportSessionRepository.GetTrackedCurrentByAdminUserAsync(
            admin.Id,
            cancellationToken);

        if (activeSession is not null)
        {
            activeSession.End(userContext.Email ?? userContext.IdentityProviderUserId ?? "system");
            await supportSessionRepository.UpdateAsync(activeSession, cancellationToken);

            await auditService.RegisterAsync(new AuditLogRequest(
                activeSession.TargetTenantId,
                admin.Id,
                AuditActions.SupportSessionEnded,
                nameof(SupportSession),
                activeSession.Id,
                "Sessão de suporte encerrada automaticamente ao iniciar nova sessão."), cancellationToken);
        }

        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        var session = new SupportSession(
            admin.Id,
            admin.IdentityProviderUserId,
            request.TenantId,
            request.Reason.Trim(),
            createdBy);

        await supportSessionRepository.AddAsync(session, cancellationToken);

        await auditService.RegisterAsync(new AuditLogRequest(
            request.TenantId,
            admin.Id,
            AuditActions.SupportSessionStarted,
            nameof(SupportSession),
            session.Id,
            $"Sessão de suporte iniciada. Motivo: {request.Reason.Trim()}"), cancellationToken);

        var refreshed = await supportSessionRepository.GetCurrentByAdminUserAsync(admin.Id, cancellationToken);

        return Result<SupportSessionResponse>.Ok(
            AdminMappingHelper.MapSupportSession(refreshed ?? session));
    }
}

public sealed class GetCurrentSupportSessionUseCase(
    ISupportSessionRepository supportSessionRepository,
    IUserProfileRepository userProfileRepository,
    IUserContext userContext) : IGetCurrentSupportSessionUseCase
{
    public async Task<Result<SupportSessionResponse?>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<SupportSessionResponse?>.Forbidden();

        var admin = await userProfileRepository.GetByIdentityProviderUserIdAsync(
            userContext.IdentityProviderUserId!,
            cancellationToken);

        if (admin is null)
            return Result<SupportSessionResponse?>.Failure("Perfil administrativo não encontrado.");

        var session = await supportSessionRepository.GetCurrentByAdminUserAsync(admin.Id, cancellationToken);

        return Result<SupportSessionResponse?>.Ok(
            session is null ? null : AdminMappingHelper.MapSupportSession(session));
    }
}

public sealed class EndCurrentSupportSessionUseCase(
    ISupportSessionRepository supportSessionRepository,
    IUserProfileRepository userProfileRepository,
    IAuditService auditService,
    IUserContext userContext) : IEndCurrentSupportSessionUseCase
{
    public async Task<Result<SupportSessionResponse?>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<SupportSessionResponse?>.Forbidden();

        var admin = await userProfileRepository.GetByIdentityProviderUserIdAsync(
            userContext.IdentityProviderUserId!,
            cancellationToken);

        if (admin is null)
            return Result<SupportSessionResponse?>.Failure("Perfil administrativo não encontrado.");

        var session = await supportSessionRepository.GetTrackedCurrentByAdminUserAsync(admin.Id, cancellationToken);

        if (session is null)
            return Result<SupportSessionResponse?>.Ok(null);

        session.End(userContext.Email ?? userContext.IdentityProviderUserId ?? "system");
        await supportSessionRepository.UpdateAsync(session, cancellationToken);

        await auditService.RegisterAsync(new AuditLogRequest(
            session.TargetTenantId,
            admin.Id,
            AuditActions.SupportSessionEnded,
            nameof(SupportSession),
            session.Id,
            "Sessão de suporte encerrada."), cancellationToken);

        var refreshed = await supportSessionRepository.GetCurrentByAdminUserAsync(admin.Id, cancellationToken);

        return Result<SupportSessionResponse?>.Ok(
            refreshed is null ? AdminMappingHelper.MapSupportSession(session) : AdminMappingHelper.MapSupportSession(refreshed));
    }
}

public sealed class GetSupportSessionsUseCase(
    ISupportSessionRepository supportSessionRepository,
    IUserContext userContext) : IGetSupportSessionsUseCase
{
    public async Task<Result<PagedResponse<SupportSessionResponse>>> ExecuteAsync(
        FilterRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<SupportSessionResponse>>.Forbidden();

        var paged = await supportSessionRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        var mapped = paged.Items.Select(AdminMappingHelper.MapSupportSession).ToList();

        return Result<PagedResponse<SupportSessionResponse>>.Ok(new PagedResponse<SupportSessionResponse>(
            mapped,
            paged.Page,
            paged.PageSize,
            paged.TotalItems,
            paged.TotalPages));
    }
}
