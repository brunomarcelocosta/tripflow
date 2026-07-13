using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Application.DTOs.Responses.Customers;

namespace Tripflow.Application.UseCases.Customers.Interfaces;

public interface ICreateCustomerUseCase
{
    Task<Result<CustomerResponse?>> ExecuteAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
}
