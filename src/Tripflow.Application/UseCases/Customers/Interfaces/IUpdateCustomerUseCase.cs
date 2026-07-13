using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Application.DTOs.Responses.Customers;

namespace Tripflow.Application.UseCases.Customers.Interfaces;

public interface IUpdateCustomerUseCase
{
    Task<Result<CustomerResponse?>> ExecuteAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default);
}
