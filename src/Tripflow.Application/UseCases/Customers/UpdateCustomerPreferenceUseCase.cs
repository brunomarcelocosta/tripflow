using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Application.DTOs.Responses.Customers;
using Tripflow.Application.UseCases.Customers.Interfaces;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Customers;

public class UpdateCustomerPreferenceUseCase(
    ICustomerRepository customerRepository,
    ICustomerPreferenceRepository preferenceRepository,
    IValidator<UpdateCustomerPreferenceRequest> validator,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IMapper mapper,
    ILogger<UpdateCustomerPreferenceUseCase> logger) : IUpdateCustomerPreferenceUseCase
{
    public string ClassName = nameof(UpdateCustomerPreferenceUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<CustomerPreferenceResponse?>> ExecuteAsync(Guid customerId, UpdateCustomerPreferenceRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<CustomerPreferenceResponse?>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<CustomerPreferenceResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.CustomersWrite,
                cancellationToken))
        {
            return Result<CustomerPreferenceResponse?>.Forbidden();
        }

        var tenantId = tenantContext.TenantId.Value;
        var actor = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("{ClassName} | {Method} | Erro de validação | CustomerId={CustomerId}", ClassName, Method, customerId);

            var firstError = validationResult.Errors.FirstOrDefault();

            return Result<CustomerPreferenceResponse?>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        var customer = await customerRepository.GetByIdAndTenantAsync(customerId, tenantId, cancellationToken);

        if (customer is null)
            return Result<CustomerPreferenceResponse?>.Failure("Cliente não encontrado.");

        try
        {
            var preference = await preferenceRepository.GetTrackedByCustomerAndTenantAsync(customerId, tenantId, cancellationToken);

            if (preference is null)
            {
                preference = new CustomerPreference(tenantId, customerId, actor);
                preference.Update(
                    request.PreferredAirlines,
                    request.PreferredHotelCategories,
                    request.SeatPreferences,
                    request.MealRestrictions,
                    request.TravelPreferences,
                    request.GeneralNotes,
                    actor);

                await preferenceRepository.AddAsync(preference, cancellationToken);

                logger.LogInformation("{ClassName} | {Method} | Preferência criada com sucesso | CustomerId={CustomerId}", ClassName, Method, customerId);
            }
            else
            {
                preference.Update(
                    request.PreferredAirlines,
                    request.PreferredHotelCategories,
                    request.SeatPreferences,
                    request.MealRestrictions,
                    request.TravelPreferences,
                    request.GeneralNotes,
                    actor);

                await preferenceRepository.UpdateAsync(preference, cancellationToken);

                logger.LogInformation("{ClassName} | {Method} | Preferência atualizada com sucesso | CustomerId={CustomerId}", ClassName, Method, customerId);
            }

            var response = mapper.Map<CustomerPreferenceResponse>(preference);

            return Result<CustomerPreferenceResponse?>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError("{ClassName} | {Method} | Erro inesperado ao atualizar preferência | CustomerId={CustomerId} | {Message}",
                ClassName, Method, customerId, ex.Message);

            return Result<CustomerPreferenceResponse?>.Failure("Erro inesperado ao atualizar preferência do cliente.");
        }
    }
}
