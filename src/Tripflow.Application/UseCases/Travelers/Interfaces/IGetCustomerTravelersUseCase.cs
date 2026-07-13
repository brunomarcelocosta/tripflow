using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Travelers;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Travelers;

namespace Tripflow.Application.UseCases.Travelers.Interfaces;

public interface IGetCustomerTravelersUseCase
{
    Task<Result<PagedResponse<TravelerResponse>>> ExecuteAsync(Guid customerId, TravelerFilterRequest request, CancellationToken cancellationToken = default);
}
