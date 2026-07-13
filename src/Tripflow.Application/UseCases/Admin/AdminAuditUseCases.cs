using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Admin;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Admin.Interfaces;
using Tripflow.Domain.Interfaces.Admin;
using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.Application.UseCases.Admin;

public sealed class GetAdminAuditLogsUseCase(
    IPlatformAdminRepository platformAdminRepository,
    IUserContext userContext) : IGetAdminAuditLogsUseCase
{
    public async Task<Result<PagedResponse<AdminAuditLogResponse>>> ExecuteAsync(
        AdminAuditLogFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<AdminAuditLogResponse>>.Forbidden();

        var paged = await platformAdminRepository.GetAuditLogsPagedAsync(
            request.ToExpression(),
            request.Page,
            request.PageSize,
            cancellationToken);

        var mapped = paged.Items.Select(AdminMappingHelper.MapAuditLog).ToList();

        return Result<PagedResponse<AdminAuditLogResponse>>.Ok(new PagedResponse<AdminAuditLogResponse>(
            mapped,
            paged.Page,
            paged.PageSize,
            paged.TotalItems,
            paged.TotalPages));
    }
}

public sealed class GetAdminAuditLogByIdUseCase(
    IPlatformAdminRepository platformAdminRepository,
    IUserContext userContext) : IGetAdminAuditLogByIdUseCase
{
    public async Task<Result<AdminAuditLogResponse?>> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<AdminAuditLogResponse?>.Forbidden();

        var auditLog = await platformAdminRepository.GetAuditLogByIdAsync(id, cancellationToken);

        return Result<AdminAuditLogResponse?>.Ok(
            auditLog is null ? null : AdminMappingHelper.MapAuditLog(auditLog));
    }
}
