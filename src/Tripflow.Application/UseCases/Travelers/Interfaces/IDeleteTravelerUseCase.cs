using Tripflow.Application.DTOs.Common;

namespace Tripflow.Application.UseCases.Travelers.Interfaces;

public interface IDeleteTravelerUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid customerId, Guid travelerId, CancellationToken cancellationToken = default);
}
