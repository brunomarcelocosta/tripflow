using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Customers;
using Tripflow.Application.UseCases.Customers.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Customers;

public class BlockCustomerUseCase(
    ICustomerRepository repository,
    ITravelerRepository travelerRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<BlockCustomerUseCase> logger) : IBlockCustomerUseCase
{
    public string ClassName = nameof(BlockCustomerUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<CustomerResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<CustomerResponse?>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<CustomerResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.CustomersWrite,
                cancellationToken))
        {
            return Result<CustomerResponse?>.Forbidden();
        }

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var customer = await repository.GetTrackedByIdAndTenantAsync(id, tenantId, cancellationToken);

        if (customer is null)
            return Result<CustomerResponse?>.Failure("Cliente não encontrado.");

        try
        {
            customer.Block(updatedBy);

            await repository.UpdateAsync(customer, cancellationToken);

            logger.LogInformation("{ClassName} | {Method} | Cliente bloqueado com sucesso | {Id}", ClassName, Method, id);

            var counts = await travelerRepository.CountByCustomersAsync(
                tenantId,
                new[] { customer.Id },
                cancellationToken);

            var travelersCount = counts.TryGetValue(customer.Id, out var count) ? count : 0;

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
        catch (Exception ex)
        {
            logger.LogError("{ClassName} | {Method} | Erro inesperado ao bloquear cliente | Id={Id} | {Message}",
                ClassName, Method, id, ex.Message);

            return Result<CustomerResponse?>.Failure("Erro inesperado ao bloquear cliente.");
        }
    }
}
