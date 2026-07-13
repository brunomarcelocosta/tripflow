using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.DTOs.Responses.Quotes;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class CreateQuoteFlightItemUseCase(
    IQuoteRepository quoteRepository,
    IQuoteFlightItemRepository flightRepository,
    IFlightSegmentRepository segmentRepository,
    IValidator<CreateQuoteFlightItemRequest> validator,
    IValidator<CreateFlightSegmentRequest> segmentValidator,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<CreateQuoteFlightItemUseCase> logger) : ICreateQuoteFlightItemUseCase
{
    public async Task<Result<QuoteFlightItemResponse?>> ExecuteAsync(Guid quoteId, CreateQuoteFlightItemRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<QuoteFlightItemResponse?>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<QuoteFlightItemResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<QuoteFlightItemResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<QuoteFlightItemResponse?>.Failure(validation.Errors.First().ErrorMessage);

        if (request.Segments is not null)
        {
            foreach (var seg in request.Segments)
            {
                var segValidation = await segmentValidator.ValidateAsync(seg, cancellationToken);
                if (!segValidation.IsValid)
                    return Result<QuoteFlightItemResponse?>.Failure(segValidation.Errors.First().ErrorMessage);
            }
        }

        var tenantId = tenantContext.TenantId.Value;
        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<QuoteFlightItemResponse?>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<QuoteFlightItemResponse?>.Failure("Cotação não pode ser alterada no status atual.");

        try
        {
            var flightItem = new QuoteFlightItem(
                tenantId, quoteId, request.AirlineName, request.FareFamily, request.BaggageDescription,
                request.IncludedPersonalItem, request.IncludedCarryOn, request.CarryOnWeightKg,
                request.IncludedCheckedBag, request.CheckedBagWeightKg, createdBy);
            await flightRepository.AddAsync(flightItem, cancellationToken);

            if (request.Segments is not null)
            {
                foreach (var seg in request.Segments)
                {
                    var segment = new FlightSegment(
                        tenantId, flightItem.Id,
                        seg.OriginAirport, seg.DestinationAirport,
                        seg.OriginCity, seg.DestinationCity,
                        seg.DepartureDateTimeUtc, seg.ArrivalDateTimeUtc,
                        seg.FlightNumber, seg.AirlineName,
                        seg.Sequence, createdBy);
                    await segmentRepository.AddAsync(segment, cancellationToken);
                }
            }

            var fresh = await flightRepository.GetByIdAndQuoteAsync(flightItem.Id, quoteId, tenantId, cancellationToken);
            return Result<QuoteFlightItemResponse?>.Ok(mapper.Map<QuoteFlightItemResponse>(fresh!));
        }
        catch (Exception ex)
        {
            logger.LogError("CreateQuoteFlightItemUseCase | Erro | {Message}", ex.Message);
            return Result<QuoteFlightItemResponse?>.Failure("Erro inesperado ao criar voo.");
        }
    }
}
