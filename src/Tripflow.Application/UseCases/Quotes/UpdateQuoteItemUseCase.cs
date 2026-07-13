using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.DTOs.Responses.Quotes;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class UpdateQuoteItemUseCase(
    IQuoteRepository quoteRepository,
    IQuoteItemRepository itemRepository,
    IValidator<UpdateQuoteItemRequest> validator,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<UpdateQuoteItemUseCase> logger) : IUpdateQuoteItemUseCase
{
    public async Task<Result<QuoteItemResponse?>> ExecuteAsync(Guid quoteId, Guid itemId, UpdateQuoteItemRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<QuoteItemResponse?>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<QuoteItemResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<QuoteItemResponse?>.Forbidden();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result<QuoteItemResponse?>.Failure(validationResult.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<QuoteItemResponse?>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<QuoteItemResponse?>.Failure("Cotação não pode ser alterada no status atual.");

        var item = await itemRepository.GetTrackedByIdAndQuoteAsync(itemId, quoteId, tenantId, cancellationToken);
        if (item is null)
            return Result<QuoteItemResponse?>.Failure("Item não encontrado.");

        try
        {
            item.Update(request.Type, request.Title, request.Description, request.AgencyCost, request.SaleAmount, request.Notes, updatedBy);
            await itemRepository.UpdateAsync(item, cancellationToken);
            return Result<QuoteItemResponse?>.Ok(mapper.Map<QuoteItemResponse>(item));
        }
        catch (Exception ex)
        {
            logger.LogError("UpdateQuoteItemUseCase | Erro | {Message}", ex.Message);
            return Result<QuoteItemResponse?>.Failure("Erro inesperado ao atualizar item.");
        }
    }
}
