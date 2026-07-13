using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Customers;
using Tripflow.Application.UseCases.Customers.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Customers;

public class GetCustomerPreferenceUseCase(
    ICustomerRepository customerRepository,
    ICustomerPreferenceRepository preferenceRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IMapper mapper) : IGetCustomerPreferenceUseCase
{
    public async Task<Result<CustomerPreferenceResponse?>> ExecuteAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<CustomerPreferenceResponse?>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<CustomerPreferenceResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.CustomersRead,
                cancellationToken))
        {
            return Result<CustomerPreferenceResponse?>.Forbidden();
        }

        var tenantId = tenantContext.TenantId.Value;

        var customer = await customerRepository.GetByIdAndTenantAsync(customerId, tenantId, cancellationToken);

        if (customer is null)
            return Result<CustomerPreferenceResponse?>.Failure("Cliente não encontrado.");

        var preference = await preferenceRepository.GetByCustomerAndTenantAsync(customerId, tenantId, cancellationToken);

        if (preference is null)
        {
            var empty = new CustomerPreferenceResponse(
                Id: Guid.Empty,
                TenantId: tenantId,
                CustomerId: customerId,
                PreferredAirlines: null,
                PreferredHotelCategories: null,
                SeatPreferences: null,
                MealRestrictions: null,
                TravelPreferences: null,
                GeneralNotes: null,
                CreatedAtUtc: default,
                UpdatedAtUtc: null);

            return Result<CustomerPreferenceResponse?>.Ok(empty);
        }

        var response = mapper.Map<CustomerPreferenceResponse>(preference);

        return Result<CustomerPreferenceResponse?>.Ok(response);
    }
}
