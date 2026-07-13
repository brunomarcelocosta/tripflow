using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.DTOs.Responses.Quotes;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class UpdateItineraryStopUseCase(
    IQuoteRepository quoteRepository,
    IItineraryRepository itineraryRepository,
    IItineraryStopRepository stopRepository,
    IValidator<UpdateItineraryStopRequest> validator,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<UpdateItineraryStopUseCase> logger) : IUpdateItineraryStopUseCase
{
    public async Task<Result<ItineraryStopResponse?>> ExecuteAsync(Guid quoteId, Guid stopId, UpdateItineraryStopRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<ItineraryStopResponse?>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<ItineraryStopResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<ItineraryStopResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<ItineraryStopResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<ItineraryStopResponse?>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<ItineraryStopResponse?>.Failure("Cotação não pode ser alterada no status atual.");

        var itinerary = await itineraryRepository.GetTrackedByQuoteIdAsync(quoteId, tenantId, cancellationToken);
        if (itinerary is null)
            return Result<ItineraryStopResponse?>.Failure("Roteiro não encontrado.");

        var stop = await stopRepository.GetTrackedByIdAndItineraryAsync(stopId, itinerary.Id, tenantId, cancellationToken);
        if (stop is null)
            return Result<ItineraryStopResponse?>.Failure("Parada não encontrada.");

        try
        {
            stop.Update(request.Country, request.City, request.Nights, request.Sequence, request.Notes, updatedBy);
            await stopRepository.UpdateAsync(stop, cancellationToken);

            var stops = await stopRepository.GetTrackedByItineraryIdAsync(itinerary.Id, tenantId, cancellationToken);
            itinerary.RecalculateTotalsFromStops(stops, updatedBy);
            await itineraryRepository.UpdateAsync(itinerary, cancellationToken);

            return Result<ItineraryStopResponse?>.Ok(mapper.Map<ItineraryStopResponse>(stop));
        }
        catch (Exception ex)
        {
            logger.LogError("UpdateItineraryStopUseCase | Erro | {Message}", ex.Message);
            return Result<ItineraryStopResponse?>.Failure("Erro inesperado ao atualizar parada.");
        }
    }
}
