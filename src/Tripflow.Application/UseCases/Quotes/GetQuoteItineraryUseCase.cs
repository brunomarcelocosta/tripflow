using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Quotes;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class GetQuoteItineraryUseCase(
    IQuoteRepository quoteRepository,
    IItineraryRepository itineraryRepository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetQuoteItineraryUseCase
{
    public async Task<Result<ItineraryResponse?>> ExecuteAsync(Guid quoteId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<ItineraryResponse?>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<ItineraryResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesRead, cancellationToken))
            return Result<ItineraryResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<ItineraryResponse?>.Failure("Cotação não encontrada.");

        var itinerary = await itineraryRepository.GetByQuoteIdAsync(quoteId, tenantId, cancellationToken);
        if (itinerary is null)
            return Result<ItineraryResponse?>.Ok(null);

        return Result<ItineraryResponse?>.Ok(mapper.Map<ItineraryResponse>(itinerary));
    }
}
