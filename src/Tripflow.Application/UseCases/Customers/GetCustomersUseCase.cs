using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Customers;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Customers.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Customers;

public class GetCustomersUseCase(
    ICustomerRepository repository,
    ITravelerRepository travelerRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetCustomersUseCase
{
    public async Task<Result<PagedResponse<CustomerResponse>>> ExecuteAsync(CustomerFilterRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<CustomerResponse>>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<PagedResponse<CustomerResponse>>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.CustomersRead,
                cancellationToken))
        {
            return Result<PagedResponse<CustomerResponse>>.Forbidden();
        }

        var tenantId = tenantContext.TenantId.Value;

        var orderBy = CustomerOrderByHelper.Build(request.SortBy);
        var filter = request.ToExpression();

        var paged = await repository.GetPagedAsync(
            filter,
            request.Page,
            request.PageSize,
            orderBy,
            request.SortDesc);

        var customerIds = paged.Items.Select(x => x.Id).ToList();

        var travelerCounts = await travelerRepository.CountByCustomersAsync(
            tenantId,
            customerIds,
            cancellationToken);

        var mapped = paged.Items.Select(c => new CustomerResponse(
            c.Id,
            c.TenantId,
            c.Type,
            c.FullName,
            c.DocumentNumber,
            c.Email,
            c.Phone,
            c.BirthDate,
            c.Notes,
            c.Status,
            travelerCounts.TryGetValue(c.Id, out var count) ? count : 0,
            c.CreatedAtUtc,
            c.UpdatedAtUtc)).ToList();

        var response = new PagedResponse<CustomerResponse>(
            mapped,
            paged.Page,
            paged.PageSize,
            paged.TotalItems,
            paged.TotalPages);

        return Result<PagedResponse<CustomerResponse>>.Ok(response);
    }
}
