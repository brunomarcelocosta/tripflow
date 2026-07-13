using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Customers;

namespace Tripflow.Application.UseCases.Customers.Interfaces;

public interface IGetCustomerPreferenceUseCase
{
    Task<Result<CustomerPreferenceResponse?>> ExecuteAsync(Guid customerId, CancellationToken cancellationToken = default);
}
