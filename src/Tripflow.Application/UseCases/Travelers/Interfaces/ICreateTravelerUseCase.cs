using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Travelers;
using Tripflow.Application.DTOs.Responses.Travelers;

namespace Tripflow.Application.UseCases.Travelers.Interfaces;

public interface ICreateTravelerUseCase
{
    Task<Result<TravelerResponse?>> ExecuteAsync(Guid customerId, CreateTravelerRequest request, CancellationToken cancellationToken = default);
}
