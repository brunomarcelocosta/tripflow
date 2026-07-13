using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Subscriptions;

namespace Tripflow.Application.UseCases.Leads.Interfaces;

public interface ICreateLeadUseCase
{
    Task<Result<LeadResponse?>> ExecuteAsync(CreateLeadRequest request, CancellationToken cancellationToken = default);
}

public interface IGetLeadsUseCase
{
    Task<Result<PagedResponse<LeadResponse>>> ExecuteAsync(LeadFilterRequest request, CancellationToken cancellationToken = default);
}

public interface IGetLeadByIdUseCase
{
    Task<Result<LeadResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IConvertLeadToTenantUseCase
{
    Task<Result<Guid?>> ExecuteAsync(Guid leadId, ConvertLeadToTenantRequest request, CancellationToken cancellationToken = default);
}

public interface IDeleteLeadUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid leadId, CancellationToken cancellationToken = default);
}
