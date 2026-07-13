using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class DeleteItineraryStopUseCase(
    IQuoteRepository quoteRepository,
    IItineraryRepository itineraryRepository,
    IItineraryStopRepository stopRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<DeleteItineraryStopUseCase> logger) : IDeleteItineraryStopUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid quoteId, Guid stopId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<bool>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<bool>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var deletedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<bool>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<bool>.Failure("Cotação não pode ser alterada no status atual.");

        var itinerary = await itineraryRepository.GetTrackedByQuoteIdAsync(quoteId, tenantId, cancellationToken);
        if (itinerary is null)
            return Result<bool>.Failure("Roteiro não encontrado.");

        var stop = await stopRepository.GetTrackedByIdAndItineraryAsync(stopId, itinerary.Id, tenantId, cancellationToken);
        if (stop is null)
            return Result<bool>.Failure("Parada não encontrada.");

        try
        {
            stop.SetDelete(deletedBy);
            await stopRepository.UpdateAsync(stop, cancellationToken);

            var stops = await stopRepository.GetTrackedByItineraryIdAsync(itinerary.Id, tenantId, cancellationToken);
            itinerary.RecalculateTotalsFromStops(stops, deletedBy);
            await itineraryRepository.UpdateAsync(itinerary, cancellationToken);

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("DeleteItineraryStopUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao remover parada.");
        }
    }
}
