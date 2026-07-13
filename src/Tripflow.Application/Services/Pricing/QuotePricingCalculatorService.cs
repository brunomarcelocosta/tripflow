using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.Services.Pricing;

public class QuotePricingCalculatorService(IPaymentFeeRuleRepository paymentFeeRuleRepository)
    : IQuotePricingCalculatorService
{
    public PricingBaseAmounts CalculateBaseAmounts(
        decimal agencyCost,
        decimal? desiredProfitAmount,
        decimal? desiredProfitPercentage,
        decimal? pixDiscountPercentage,
        TenantCommercialSettings? commercialSettings)
    {
        decimal creditCashAmount;

        if (desiredProfitAmount.HasValue)
        {
            creditCashAmount = agencyCost + desiredProfitAmount.Value;
        }
        else if (desiredProfitPercentage.HasValue)
        {
            creditCashAmount = agencyCost + (agencyCost * desiredProfitPercentage.Value / 100m);
        }
        else if (commercialSettings?.DefaultProfitAmount.HasValue == true)
        {
            creditCashAmount = agencyCost + commercialSettings.DefaultProfitAmount.Value;
        }
        else if (commercialSettings?.DefaultProfitPercentage.HasValue == true)
        {
            creditCashAmount = agencyCost + (agencyCost * commercialSettings.DefaultProfitPercentage.Value / 100m);
        }
        else
        {
            creditCashAmount = agencyCost;
        }

        var effectivePixDiscount = pixDiscountPercentage
            ?? commercialSettings?.DefaultPixDiscountPercentage
            ?? 0m;

        var pixAmount = creditCashAmount - (creditCashAmount * effectivePixDiscount / 100m);

        return new PricingBaseAmounts(
            Math.Round(creditCashAmount, 2),
            Math.Round(pixAmount, 2),
            effectivePixDiscount);
    }

    public async Task<IReadOnlyList<QuotePaymentCondition>> GeneratePaymentConditionsAsync(
        QuotePricingOption pricingOption,
        string createdBy,
        CancellationToken cancellationToken = default)
    {
        var rules = await paymentFeeRuleRepository.GetByTenantIdAsync(pricingOption.TenantId, cancellationToken);

        var activeRules = rules.Where(r => r.IsActive).ToList();

        if (activeRules.Count == 0)
            return Array.Empty<QuotePaymentCondition>();

        var conditions = new List<QuotePaymentCondition>();
        var agencyCost = pricingOption.AgencyCost;
        var creditCash = pricingOption.CreditCashAmount ?? pricingOption.AgencyCost;
        var pix = pricingOption.PixAmount ?? creditCash;

        foreach (var rule in activeRules)
        {
            switch (rule.PaymentMethod)
            {
                case PaymentMethod.Pix:
                {
                    var gross = Math.Round(pix, 2);
                    var fee = Math.Round(gross * rule.FeePercentage / 100m, 2);
                    var net = Math.Round(gross - fee, 2);
                    var profit = Math.Round(net - agencyCost, 2);

                    conditions.Add(new QuotePaymentCondition(
                        pricingOption.TenantId,
                        pricingOption.Id,
                        PaymentMethod.Pix,
                        1,
                        rule.FeePercentage,
                        gross,
                        gross,
                        fee,
                        net,
                        profit,
                        createdBy));
                    break;
                }
                case PaymentMethod.CreditCard:
                {
                    decimal gross;
                    if (rule.FeePercentage >= 100m)
                        continue;

                    if (rule.FeePercentage == 0m)
                        gross = creditCash;
                    else
                        gross = creditCash / (1m - rule.FeePercentage / 100m);

                    gross = Math.Round(gross, 2);
                    var installmentAmount = rule.Installments > 0 ? Math.Round(gross / rule.Installments, 2) : (decimal?)null;
                    var fee = Math.Round(gross * rule.FeePercentage / 100m, 2);
                    var net = Math.Round(gross - fee, 2);
                    var profit = Math.Round(net - agencyCost, 2);

                    conditions.Add(new QuotePaymentCondition(
                        pricingOption.TenantId,
                        pricingOption.Id,
                        PaymentMethod.CreditCard,
                        rule.Installments,
                        rule.FeePercentage,
                        gross,
                        installmentAmount,
                        fee,
                        net,
                        profit,
                        createdBy));
                    break;
                }
                case PaymentMethod.Manual:
                {
                    var gross = Math.Round(creditCash, 2);
                    var fee = Math.Round(gross * rule.FeePercentage / 100m, 2);
                    var net = Math.Round(gross - fee, 2);
                    var profit = Math.Round(net - agencyCost, 2);

                    conditions.Add(new QuotePaymentCondition(
                        pricingOption.TenantId,
                        pricingOption.Id,
                        PaymentMethod.Manual,
                        Math.Max(rule.Installments, 1),
                        rule.FeePercentage,
                        gross,
                        gross,
                        fee,
                        net,
                        profit,
                        createdBy));
                    break;
                }
                default:
                    continue;
            }
        }

        return conditions;
    }
}
