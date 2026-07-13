using Tripflow.Application.DTOs.Common;

namespace Tripflow.Application.UseCases.Customers.Interfaces;

public interface IDeleteCustomerUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}
