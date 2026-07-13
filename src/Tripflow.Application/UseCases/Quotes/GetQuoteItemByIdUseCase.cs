using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Quotes;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class GetQuoteItemByIdUseCase(
    IQuoteRepository quoteRepository,
    IQuoteItemRepository itemRepository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetQuoteItemByIdUseCase
{
    public async Task<Result<QuoteItemResponse?>> ExecuteAsync(Guid quoteId, Guid itemId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<QuoteItemResponse?>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<QuoteItemResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesRead, cancellationToken))
            return Result<QuoteItemResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<QuoteItemResponse?>.Failure("Cotação não encontrada.");

        var item = await itemRepository.GetByIdAndQuoteAsync(itemId, quoteId, tenantId, cancellationToken);
        if (item is null)
            return Result<QuoteItemResponse?>.Ok(null);

        return Result<QuoteItemResponse?>.Ok(mapper.Map<QuoteItemResponse>(item));
    }
}
