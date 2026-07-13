using AutoMapper;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Pricing;
using Tripflow.Application.Helpers;
using Tripflow.Application.Services.Pricing;
using Tripflow.Application.UseCases.Pricing.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Pricing;

public class RegenerateQuotePaymentConditionsUseCase(
    IQuoteRepository quoteRepository,
    IQuotePricingOptionRepository pricingRepository,
    IQuotePaymentConditionRepository paymentConditionRepository,
    IQuotePricingCalculatorService calculatorService,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<RegenerateQuotePaymentConditionsUseCase> logger) : IRegenerateQuotePaymentConditionsUseCase
{
    public async Task<Result<IEnumerable<QuotePaymentConditionResponse>>> ExecuteAsync(Guid quoteId, Guid pricingOptionId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<IEnumerable<QuotePaymentConditionResponse>>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<IEnumerable<QuotePaymentConditionResponse>>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<IEnumerable<QuotePaymentConditionResponse>>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<IEnumerable<QuotePaymentConditionResponse>>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<IEnumerable<QuotePaymentConditionResponse>>.Failure("Cotação não pode ser alterada no status atual.");

        var option = await pricingRepository.GetTrackedByIdAndQuoteAsync(pricingOptionId, quoteId, tenantId, cancellationToken);
        if (option is null)
            return Result<IEnumerable<QuotePaymentConditionResponse>>.Failure("Opção de precificação não encontrada.");

        try
        {
            var conditions = await calculatorService.GeneratePaymentConditionsAsync(option, updatedBy, cancellationToken);

            await paymentConditionRepository.DeleteByPricingOptionIdAsync(option.Id, tenantId, cancellationToken);

            if (conditions.Count == 0)
                return Result<IEnumerable<QuotePaymentConditionResponse>>.Failure("Nenhuma regra financeira ativa configurada para o tenant.");

            await paymentConditionRepository.AddRangeAsync(conditions, cancellationToken);

            var fresh = await paymentConditionRepository.GetByPricingOptionIdAsync(option.Id, tenantId, cancellationToken);
            var mapped = fresh.Select(mapper.Map<QuotePaymentConditionResponse>).ToList();
            return Result<IEnumerable<QuotePaymentConditionResponse>>.Ok(mapped);
        }
        catch (Exception ex)
        {
            logger.LogError("RegenerateQuotePaymentConditionsUseCase | Erro | {Message}", ex.Message);
            return Result<IEnumerable<QuotePaymentConditionResponse>>.Failure("Erro inesperado ao regerar condições de pagamento.");
        }
    }
}
