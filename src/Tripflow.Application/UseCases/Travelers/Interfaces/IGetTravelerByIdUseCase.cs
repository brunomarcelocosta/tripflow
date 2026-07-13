using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Travelers;

namespace Tripflow.Application.UseCases.Travelers.Interfaces;

public interface IGetTravelerByIdUseCase
{
    Task<Result<TravelerResponse?>> ExecuteAsync(Guid travelerId, Guid? customerId, CancellationToken cancellationToken = default);
}
