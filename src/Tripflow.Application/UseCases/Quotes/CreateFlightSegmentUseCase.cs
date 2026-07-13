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

public class CreateFlightSegmentUseCase(
    IQuoteRepository quoteRepository,
    IQuoteFlightItemRepository flightRepository,
    IFlightSegmentRepository segmentRepository,
    IValidator<CreateFlightSegmentRequest> validator,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<CreateFlightSegmentUseCase> logger) : ICreateFlightSegmentUseCase
{
    public async Task<Result<FlightSegmentResponse?>> ExecuteAsync(Guid quoteId, Guid flightItemId, CreateFlightSegmentRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<FlightSegmentResponse?>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<FlightSegmentResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<FlightSegmentResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<FlightSegmentResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<FlightSegmentResponse?>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<FlightSegmentResponse?>.Failure("Cotação não pode ser alterada no status atual.");

        var flightItem = await flightRepository.GetByIdAndQuoteAsync(flightItemId, quoteId, tenantId, cancellationToken);
        if (flightItem is null)
            return Result<FlightSegmentResponse?>.Failure("Voo não encontrado.");

        try
        {
            var segment = new FlightSegment(
                tenantId, flightItem.Id,
                request.OriginAirport, request.DestinationAirport,
                request.OriginCity, request.DestinationCity,
                request.DepartureDateTimeUtc, request.ArrivalDateTimeUtc,
                request.FlightNumber, request.AirlineName,
                request.Sequence, createdBy);
            await segmentRepository.AddAsync(segment, cancellationToken);
            return Result<FlightSegmentResponse?>.Ok(mapper.Map<FlightSegmentResponse>(segment));
        }
        catch (Exception ex)
        {
            logger.LogError("CreateFlightSegmentUseCase | Erro | {Message}", ex.Message);
            return Result<FlightSegmentResponse?>.Failure("Erro inesperado ao criar segmento.");
        }
    }
}
