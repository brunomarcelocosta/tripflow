using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Customers;

namespace Tripflow.Application.UseCases.Customers.Interfaces;

public interface IGetCustomersUseCase
{
    Task<Result<PagedResponse<CustomerResponse>>> ExecuteAsync(CustomerFilterRequest request, CancellationToken cancellationToken = default);
}
