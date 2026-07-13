using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Customers;

namespace Tripflow.Application.UseCases.Customers.Interfaces;

public interface IInactivateCustomerUseCase
{
    Task<Result<CustomerResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}
