using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Pricing.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Pricing;

public class DeleteQuotePricingOptionUseCase(
    IQuoteRepository quoteRepository,
    IQuotePricingOptionRepository pricingRepository,
    IQuotePaymentConditionRepository paymentConditionRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<DeleteQuotePricingOptionUseCase> logger) : IDeleteQuotePricingOptionUseCase
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
        var deletedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<bool>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<bool>.Failure("Cotação não pode ser alterada no status atual.");

        var option = await pricingRepository.GetTrackedByIdAndQuoteAsync(pricingOptionId, quoteId, tenantId, cancellationToken);
        if (option is null)
            return Result<bool>.Failure("Opção de precificação não encontrada.");

        try
        {
            await paymentConditionRepository.DeleteByPricingOptionIdAsync(option.Id, tenantId, cancellationToken);
            option.SetDelete(deletedBy);
            await pricingRepository.UpdateAsync(option, cancellationToken);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("DeleteQuotePricingOptionUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao remover opção de precificação.");
        }
    }
}
