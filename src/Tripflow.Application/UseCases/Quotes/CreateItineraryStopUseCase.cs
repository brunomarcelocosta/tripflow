using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.DTOs.Responses.Quotes;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Entities;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class CreateItineraryStopUseCase(
    IQuoteRepository quoteRepository,
    IItineraryRepository itineraryRepository,
    IItineraryStopRepository stopRepository,
    IValidator<CreateItineraryStopRequest> validator,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<CreateItineraryStopUseCase> logger) : ICreateItineraryStopUseCase
{
    public async Task<Result<ItineraryStopResponse?>> ExecuteAsync(Guid quoteId, CreateItineraryStopRequest request, CancellationToken cancellationToken = default)
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
        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<ItineraryStopResponse?>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<ItineraryStopResponse?>.Failure("Cotação não pode ser alterada no status atual.");

        try
        {
            var itinerary = await itineraryRepository.GetTrackedByQuoteIdAsync(quoteId, tenantId, cancellationToken);
            if (itinerary is null)
            {
                itinerary = new Itinerary(tenantId, quoteId, quote.Title, null, null, createdBy);
                await itineraryRepository.AddAsync(itinerary, cancellationToken);
            }

            var stop = new ItineraryStop(tenantId, itinerary.Id, request.Country, request.City, request.Nights, request.Sequence, request.Notes, createdBy);
            await stopRepository.AddAsync(stop, cancellationToken);

            var stops = await stopRepository.GetTrackedByItineraryIdAsync(itinerary.Id, tenantId, cancellationToken);
            itinerary.RecalculateTotalsFromStops(stops, createdBy);
            await itineraryRepository.UpdateAsync(itinerary, cancellationToken);

            return Result<ItineraryStopResponse?>.Ok(mapper.Map<ItineraryStopResponse>(stop));
        }
        catch (Exception ex)
        {
            logger.LogError("CreateItineraryStopUseCase | Erro | {Message}", ex.Message);
            return Result<ItineraryStopResponse?>.Failure("Erro inesperado ao adicionar parada.");
        }
    }
}
