using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Pricing.Interfaces;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Pricing;

public class SelectQuotePricingOptionUseCase(
    IQuoteRepository quoteRepository,
    IQuotePricingOptionRepository pricingRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<SelectQuotePricingOptionUseCase> logger) : ISelectQuotePricingOptionUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid quoteId, Guid pricingOptionId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<bool>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<bool>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetTrackedByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<bool>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<bool>.Failure("Cotação não pode ser alterada no status atual.");

        var options = await pricingRepository.GetTrackedByQuoteIdAsync(quoteId, tenantId, cancellationToken);
        var target = options.FirstOrDefault(o => o.Id == pricingOptionId);
        if (target is null)
            return Result<bool>.Failure("Opção de precificação não encontrada.");

        try
        {
            foreach (var other in options.Where(o => o.Id != pricingOptionId && o.Selected))
            {
                other.Unselect(updatedBy);
                await pricingRepository.UpdateAsync(other, cancellationToken);
            }

            target.MarkAsSelected(updatedBy);
            await pricingRepository.UpdateAsync(target, cancellationToken);

            if (quote.Status == QuoteStatus.Draft)
            {
                quote.MarkAsCalculated(updatedBy);
                await quoteRepository.UpdateAsync(quote, cancellationToken);
            }

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("SelectQuotePricingOptionUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao selecionar opção de precificação.");
        }
    }
}
