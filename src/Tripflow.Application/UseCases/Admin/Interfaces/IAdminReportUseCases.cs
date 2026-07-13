using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Admin;

namespace Tripflow.Application.UseCases.Admin.Interfaces;

public interface IGetAdminTenantReportUseCase
{
    Task<Result<PagedResponse<AdminTenantReportResponse>>> ExecuteAsync(AdminReportFilterRequest request, CancellationToken cancellationToken = default);
}

public interface IGetAdminUsageReportUseCase
{
    Task<Result<PagedResponse<AdminUsageReportResponse>>> ExecuteAsync(AdminReportFilterRequest request, CancellationToken cancellationToken = default);
}

public interface IGetAdminPaymentReportUseCase
{
    Task<Result<PagedResponse<AdminPaymentReportResponse>>> ExecuteAsync(AdminReportFilterRequest request, CancellationToken cancellationToken = default);
}

public interface IGetAdminProposalReportUseCase
{
    Task<Result<PagedResponse<AdminProposalReportResponse>>> ExecuteAsync(AdminReportFilterRequest request, CancellationToken cancellationToken = default);
}

public interface IGetAdminSubscriptionReportUseCase
{
    Task<Result<PagedResponse<AdminSubscriptionReportResponse>>> ExecuteAsync(AdminReportFilterRequest request, CancellationToken cancellationToken = default);
}
