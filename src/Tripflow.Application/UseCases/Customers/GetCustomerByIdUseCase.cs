using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Customers;
using Tripflow.Application.UseCases.Customers.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Customers;

public class GetCustomerByIdUseCase(
    ICustomerRepository repository,
    ITravelerRepository travelerRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetCustomerByIdUseCase
{
    public async Task<Result<CustomerResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<CustomerResponse?>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<CustomerResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.CustomersRead,
                cancellationToken))
        {
            return Result<CustomerResponse?>.Forbidden();
        }

        var tenantId = tenantContext.TenantId.Value;

        var customer = await repository.GetByIdAndTenantAsync(id, tenantId, cancellationToken);

        if (customer is null)
            return Result<CustomerResponse?>.Failure("Cliente não encontrado.");

        var counts = await travelerRepository.CountByCustomersAsync(
            tenantId,
            new[] { customer.Id },
            cancellationToken);

        var travelersCount = counts.TryGetValue(customer.Id, out var c) ? c : 0;

        var response = new CustomerResponse(
            customer.Id,
            customer.TenantId,
            customer.Type,
            customer.FullName,
            customer.DocumentNumber,
            customer.Email,
            customer.Phone,
            customer.BirthDate,
            customer.Notes,
            customer.Status,
            travelersCount,
            customer.CreatedAtUtc,
            customer.UpdatedAtUtc);

        return Result<CustomerResponse?>.Ok(response);
    }
}
