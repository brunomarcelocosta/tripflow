using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Admin;
using Tripflow.Application.Helpers;
using Tripflow.Application.Services.Audit;
using Tripflow.Application.UseCases.Admin.Interfaces;
using Tripflow.Domain.Interfaces.Admin;
using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.Application.UseCases.Admin;

public sealed class GetAdminTenantReportUseCase(
    IPlatformAdminRepository platformAdminRepository,
    IAuditService auditService,
    IUserContext userContext) : IGetAdminTenantReportUseCase
{
    public async Task<Result<PagedResponse<AdminTenantReportResponse>>> ExecuteAsync(
        AdminReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<AdminTenantReportResponse>>.Forbidden();

        var paged = await platformAdminRepository.GetTenantReportPagedAsync(
            request.ToTenantReportExpression(),
            request.Page,
            request.PageSize,
            cancellationToken);

        await auditService.RegisterAsync(new AuditLogRequest(
            null,
            null,
            AuditActions.AdminReportViewed,
            "AdminReport",
            null,
            "Relatório de tenants consultado."), cancellationToken);

        var mapped = paged.Items.Select(AdminMappingHelper.MapTenantReport).ToList();

        return Result<PagedResponse<AdminTenantReportResponse>>.Ok(new PagedResponse<AdminTenantReportResponse>(
            mapped,
            paged.Page,
            paged.PageSize,
            paged.TotalItems,
            paged.TotalPages));
    }
}

public sealed class GetAdminUsageReportUseCase(
    IPlatformAdminRepository platformAdminRepository,
    IAuditService auditService,
    IUserContext userContext) : IGetAdminUsageReportUseCase
{
    public async Task<Result<PagedResponse<AdminUsageReportResponse>>> ExecuteAsync(
        AdminReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<AdminUsageReportResponse>>.Forbidden();

        var paged = await platformAdminRepository.GetUsageReportPagedAsync(
            request.ToUsageReportExpression(),
            request.Page,
            request.PageSize,
            cancellationToken);

        await auditService.RegisterAsync(new AuditLogRequest(
            null,
            null,
            AuditActions.AdminReportViewed,
            "AdminReport",
            null,
            "Relatório de uso consultado."), cancellationToken);

        var mapped = paged.Items.Select(AdminMappingHelper.MapUsageReport).ToList();

        return Result<PagedResponse<AdminUsageReportResponse>>.Ok(new PagedResponse<AdminUsageReportResponse>(
            mapped,
            paged.Page,
            paged.PageSize,
            paged.TotalItems,
            paged.TotalPages));
    }
}

public sealed class GetAdminPaymentReportUseCase(
    IPlatformAdminRepository platformAdminRepository,
    IAuditService auditService,
    IUserContext userContext) : IGetAdminPaymentReportUseCase
{
    public async Task<Result<PagedResponse<AdminPaymentReportResponse>>> ExecuteAsync(
        AdminReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<AdminPaymentReportResponse>>.Forbidden();

        var paged = await platformAdminRepository.GetPaymentReportPagedAsync(
            request.ToPaymentReportExpression(),
            request.Page,
            request.PageSize,
            cancellationToken);

        await auditService.RegisterAsync(new AuditLogRequest(
            null,
            null,
            AuditActions.AdminReportViewed,
            "AdminReport",
            null,
            "Relatório de pagamentos consultado."), cancellationToken);

        var mapped = paged.Items.Select(AdminMappingHelper.MapPaymentReport).ToList();

        return Result<PagedResponse<AdminPaymentReportResponse>>.Ok(new PagedResponse<AdminPaymentReportResponse>(
            mapped,
            paged.Page,
            paged.PageSize,
            paged.TotalItems,
            paged.TotalPages));
    }
}

public sealed class GetAdminProposalReportUseCase(
    IPlatformAdminRepository platformAdminRepository,
    IAuditService auditService,
    IUserContext userContext) : IGetAdminProposalReportUseCase
{
    public async Task<Result<PagedResponse<AdminProposalReportResponse>>> ExecuteAsync(
        AdminReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<AdminProposalReportResponse>>.Forbidden();

        var paged = await platformAdminRepository.GetProposalReportPagedAsync(
            request.ToProposalReportExpression(),
            request.Page,
            request.PageSize,
            cancellationToken);

        await auditService.RegisterAsync(new AuditLogRequest(
            null,
            null,
            AuditActions.AdminReportViewed,
            "AdminReport",
            null,
            "Relatório de propostas consultado."), cancellationToken);

        var mapped = paged.Items.Select(AdminMappingHelper.MapProposalReport).ToList();

        return Result<PagedResponse<AdminProposalReportResponse>>.Ok(new PagedResponse<AdminProposalReportResponse>(
            mapped,
            paged.Page,
            paged.PageSize,
            paged.TotalItems,
            paged.TotalPages));
    }
}

public sealed class GetAdminSubscriptionReportUseCase(
    IPlatformAdminRepository platformAdminRepository,
    IAuditService auditService,
    IUserContext userContext) : IGetAdminSubscriptionReportUseCase
{
    public async Task<Result<PagedResponse<AdminSubscriptionReportResponse>>> ExecuteAsync(
        AdminReportFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<AdminSubscriptionReportResponse>>.Forbidden();

        var paged = await platformAdminRepository.GetSubscriptionReportPagedAsync(
            request.ToSubscriptionReportExpression(),
            request.Page,
            request.PageSize,
            cancellationToken);

        await auditService.RegisterAsync(new AuditLogRequest(
            null,
            null,
            AuditActions.AdminReportViewed,
            "AdminReport",
            null,
            "Relatório de assinaturas consultado."), cancellationToken);

        var mapped = paged.Items.Select(AdminMappingHelper.MapSubscriptionReport).ToList();

        return Result<PagedResponse<AdminSubscriptionReportResponse>>.Ok(new PagedResponse<AdminSubscriptionReportResponse>(
            mapped,
            paged.Page,
            paged.PageSize,
            paged.TotalItems,
            paged.TotalPages));
    }
}
