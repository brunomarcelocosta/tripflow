using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.UseCases.Customers.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Customers;

public class DeleteCustomerUseCase(
    ICustomerRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<DeleteCustomerUseCase> logger) : IDeleteCustomerUseCase
{
    public string ClassName = nameof(DeleteCustomerUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<bool>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.CustomersWrite,
                cancellationToken))
        {
            return Result<bool>.Forbidden();
        }

        var tenantId = tenantContext.TenantId.Value;
        var deletedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var customer = await repository.GetTrackedByIdAndTenantAsync(id, tenantId, cancellationToken);

        if (customer is null)
            return Result<bool>.Failure("Cliente não encontrado.");

        try
        {
            customer.SetDelete(deletedBy);

            await repository.UpdateAsync(customer, cancellationToken);

            logger.LogInformation("{ClassName} | {Method} | Cliente removido com sucesso | {Id}", ClassName, Method, id);

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("{ClassName} | {Method} | Erro inesperado ao remover cliente | Id={Id} | {Message}",
                ClassName, Method, id, ex.Message);

            return Result<bool>.Failure("Erro inesperado ao remover cliente.");
        }
    }
}
