using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Quotes;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class GetQuoteItemsUseCase(
    IQuoteRepository quoteRepository,
    IQuoteItemRepository itemRepository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetQuoteItemsUseCase
{
    public async Task<Result<IEnumerable<QuoteItemResponse>>> ExecuteAsync(Guid quoteId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<IEnumerable<QuoteItemResponse>>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<IEnumerable<QuoteItemResponse>>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesRead, cancellationToken))
            return Result<IEnumerable<QuoteItemResponse>>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<IEnumerable<QuoteItemResponse>>.Failure("Cotação não encontrada.");

        var items = await itemRepository.GetByQuoteIdAsync(quoteId, tenantId, cancellationToken);
        var mapped = items.Select(mapper.Map<QuoteItemResponse>).ToList();
        return Result<IEnumerable<QuoteItemResponse>>.Ok(mapped);
    }
}
