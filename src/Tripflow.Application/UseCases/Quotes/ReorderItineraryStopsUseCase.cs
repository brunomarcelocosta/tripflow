using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class ReorderItineraryStopsUseCase(
    IQuoteRepository quoteRepository,
    IItineraryRepository itineraryRepository,
    IItineraryStopRepository stopRepository,
    IValidator<ReorderItineraryStopsRequest> validator,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<ReorderItineraryStopsUseCase> logger) : IReorderItineraryStopsUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid quoteId, ReorderItineraryStopsRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<bool>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<bool>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<bool>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<bool>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<bool>.Failure("Cotação não pode ser alterada no status atual.");

        var itinerary = await itineraryRepository.GetTrackedByQuoteIdAsync(quoteId, tenantId, cancellationToken);
        if (itinerary is null)
            return Result<bool>.Failure("Roteiro não encontrado.");

        var stops = await stopRepository.GetTrackedByItineraryIdAsync(itinerary.Id, tenantId, cancellationToken);
        var byId = stops.ToDictionary(s => s.Id);

        try
        {
            foreach (var item in request.Stops)
            {
                if (!byId.TryGetValue(item.StopId, out var stop))
                    return Result<bool>.Failure($"Parada {item.StopId} não pertence ao roteiro.");
                stop.UpdateSequence(item.Sequence, updatedBy);
                await stopRepository.UpdateAsync(stop, cancellationToken);
            }
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("ReorderItineraryStopsUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao reordenar paradas.");
        }
    }
}
