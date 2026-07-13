using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Admin;

namespace Tripflow.Application.UseCases.Admin.Interfaces;

public interface IGetAdminAuditLogsUseCase
{
    Task<Result<PagedResponse<AdminAuditLogResponse>>> ExecuteAsync(AdminAuditLogFilterRequest request, CancellationToken cancellationToken = default);
}

public interface IGetAdminAuditLogByIdUseCase
{
    Task<Result<AdminAuditLogResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}
