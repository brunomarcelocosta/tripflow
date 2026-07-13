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

public class UpdateQuoteItineraryUseCase(
    IQuoteRepository quoteRepository,
    IItineraryRepository itineraryRepository,
    IValidator<UpdateItineraryRequest> validator,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<UpdateQuoteItineraryUseCase> logger) : IUpdateQuoteItineraryUseCase
{
    public async Task<Result<ItineraryResponse?>> ExecuteAsync(Guid quoteId, UpdateItineraryRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<ItineraryResponse?>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<ItineraryResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<ItineraryResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<ItineraryResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<ItineraryResponse?>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<ItineraryResponse?>.Failure("Cotação não pode ser alterada no status atual.");

        try
        {
            var itinerary = await itineraryRepository.GetTrackedByQuoteIdAsync(quoteId, tenantId, cancellationToken);

            if (itinerary is null)
            {
                itinerary = new Itinerary(tenantId, quoteId, request.Name, request.TotalDays, request.TotalNights, updatedBy);
                await itineraryRepository.AddAsync(itinerary, cancellationToken);
            }
            else
            {
                itinerary.Update(request.Name, request.TotalDays, request.TotalNights, updatedBy);
                await itineraryRepository.UpdateAsync(itinerary, cancellationToken);
            }

            var fresh = await itineraryRepository.GetByQuoteIdAsync(quoteId, tenantId, cancellationToken);
            return Result<ItineraryResponse?>.Ok(mapper.Map<ItineraryResponse>(fresh!));
        }
        catch (Exception ex)
        {
            logger.LogError("UpdateQuoteItineraryUseCase | Erro | {Message}", ex.Message);
            return Result<ItineraryResponse?>.Failure("Erro inesperado ao atualizar roteiro.");
        }
    }
}
