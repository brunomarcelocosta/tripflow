using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.UseCases.Travelers.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Travelers;

public class DeleteTravelerUseCase(
    ITravelerRepository travelerRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<DeleteTravelerUseCase> logger) : IDeleteTravelerUseCase
{
    public string ClassName = nameof(DeleteTravelerUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<bool>> ExecuteAsync(Guid customerId, Guid travelerId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<bool>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.TravelersWrite,
                cancellationToken))
        {
            return Result<bool>.Forbidden();
        }

        var tenantId = tenantContext.TenantId.Value;
        var deletedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var traveler = await travelerRepository.GetTrackedByCustomerAndTenantAsync(
            travelerId,
            customerId,
            tenantId,
            cancellationToken);

        if (traveler is null)
            return Result<bool>.Failure("Viajante não encontrado.");

        try
        {
            traveler.SetDelete(deletedBy);

            await travelerRepository.UpdateAsync(traveler, cancellationToken);

            logger.LogInformation("{ClassName} | {Method} | Viajante removido com sucesso | {Id}", ClassName, Method, travelerId);

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("{ClassName} | {Method} | Erro inesperado ao remover viajante | Id={Id} | {Message}",
                ClassName, Method, travelerId, ex.Message);

            return Result<bool>.Failure("Erro inesperado ao remover viajante.");
        }
    }
}
