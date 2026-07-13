using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Application.DTOs.Responses.Customers;

namespace Tripflow.Application.UseCases.Customers.Interfaces;

public interface IUpdateCustomerPreferenceUseCase
{
    Task<Result<CustomerPreferenceResponse?>> ExecuteAsync(Guid customerId, UpdateCustomerPreferenceRequest request, CancellationToken cancellationToken = default);
}
