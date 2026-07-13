using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Quotes;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class GetQuoteFlightsUseCase(
    IQuoteRepository quoteRepository,
    IQuoteFlightItemRepository flightRepository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetQuoteFlightsUseCase
{
    public async Task<Result<IEnumerable<QuoteFlightItemResponse>>> ExecuteAsync(Guid quoteId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<IEnumerable<QuoteFlightItemResponse>>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<IEnumerable<QuoteFlightItemResponse>>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesRead, cancellationToken))
            return Result<IEnumerable<QuoteFlightItemResponse>>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<IEnumerable<QuoteFlightItemResponse>>.Failure("Cotação não encontrada.");

        var items = await flightRepository.GetByQuoteIdAsync(quoteId, tenantId, cancellationToken);
        var mapped = items.Select(mapper.Map<QuoteFlightItemResponse>).ToList();
        return Result<IEnumerable<QuoteFlightItemResponse>>.Ok(mapped);
    }
}
