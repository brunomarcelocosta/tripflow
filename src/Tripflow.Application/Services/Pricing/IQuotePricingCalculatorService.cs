using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Entities.Tenants;

namespace Tripflow.Application.Services.Pricing;

public sealed record PricingBaseAmounts(decimal CreditCashAmount, decimal PixAmount, decimal? PixDiscountPercentage);

public interface IQuotePricingCalculatorService
{
    PricingBaseAmounts CalculateBaseAmounts(
        decimal agencyCost,
        decimal? desiredProfitAmount,
        decimal? desiredProfitPercentage,
        decimal? pixDiscountPercentage,
        TenantCommercialSettings? commercialSettings);

    Task<IReadOnlyList<QuotePaymentCondition>> GeneratePaymentConditionsAsync(
        QuotePricingOption pricingOption,
        string createdBy,
        CancellationToken cancellationToken = default);
}
