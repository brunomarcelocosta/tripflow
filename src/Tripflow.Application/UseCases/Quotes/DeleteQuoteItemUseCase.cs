using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class DeleteQuoteItemUseCase(
    IQuoteRepository quoteRepository,
    IQuoteItemRepository itemRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<DeleteQuoteItemUseCase> logger) : IDeleteQuoteItemUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid quoteId, Guid itemId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<bool>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<bool>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var deletedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<bool>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<bool>.Failure("Cotação não pode ser alterada no status atual.");

        var item = await itemRepository.GetTrackedByIdAndQuoteAsync(itemId, quoteId, tenantId, cancellationToken);
        if (item is null)
            return Result<bool>.Failure("Item não encontrado.");

        try
        {
            item.SetDelete(deletedBy);
            await itemRepository.UpdateAsync(item, cancellationToken);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("DeleteQuoteItemUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao remover item.");
        }
    }
}
