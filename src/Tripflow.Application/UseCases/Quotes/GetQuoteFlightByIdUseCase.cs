using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Quotes;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class GetQuoteFlightByIdUseCase(
    IQuoteRepository quoteRepository,
    IQuoteFlightItemRepository flightRepository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetQuoteFlightByIdUseCase
{
    public async Task<Result<QuoteFlightItemResponse?>> ExecuteAsync(Guid quoteId, Guid flightItemId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<QuoteFlightItemResponse?>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<QuoteFlightItemResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesRead, cancellationToken))
            return Result<QuoteFlightItemResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<QuoteFlightItemResponse?>.Failure("Cotação não encontrada.");

        var item = await flightRepository.GetByIdAndQuoteAsync(flightItemId, quoteId, tenantId, cancellationToken);
        if (item is null)
            return Result<QuoteFlightItemResponse?>.Failure("Voo não encontrado.");

        return Result<QuoteFlightItemResponse?>.Ok(mapper.Map<QuoteFlightItemResponse>(item));
    }
}
