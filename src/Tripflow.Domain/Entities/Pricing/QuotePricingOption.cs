using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Pricing;

public sealed class QuotePricingOption : AuditableEntity, ITenantEntity
{
    private QuotePricingOption() { }

    public QuotePricingOption(
        Guid tenantId,
        Guid quoteId,
        string name,
        decimal agencyCost,
        decimal? desiredProfitAmount,
        decimal? desiredProfitPercentage,
        decimal? pixDiscountPercentage,
        decimal? pixAmount,
        decimal? creditCashAmount,
        bool selected,
        string createdBy)
    {
        TenantId = tenantId;
        QuoteId = quoteId;
        Name = name;
        AgencyCost = agencyCost;
        DesiredProfitAmount = desiredProfitAmount;
        DesiredProfitPercentage = desiredProfitPercentage;
        PixDiscountPercentage = pixDiscountPercentage;
        PixAmount = pixAmount;
        CreditCashAmount = creditCashAmount;
        Selected = selected;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid QuoteId { get; private set; }
    public Quote Quote { get; private set; } = default!;

    public string Name { get; private set; } = default!;

    public decimal AgencyCost { get; private set; }
    public decimal? DesiredProfitAmount { get; private set; }
    public decimal? DesiredProfitPercentage { get; private set; }

    public decimal? PixDiscountPercentage { get; private set; }
    public decimal? PixAmount { get; private set; }

    public decimal? CreditCashAmount { get; private set; }
    public bool Selected { get; private set; }

    public string? InternalNotes { get; private set; }

    public List<QuotePaymentCondition> PaymentConditions = [];

    public void MarkAsSelected(string updatedBy)
    {
        Selected = true;
        SetUpdated(updatedBy);
    }

    public void Unselect(string updatedBy)
    {
        Selected = false;
        SetUpdated(updatedBy);
    }

    public void UpdateAmounts(
        decimal agencyCost,
        decimal? desiredProfitAmount,
        decimal? desiredProfitPercentage,
        decimal? pixDiscountPercentage,
        decimal? pixAmount,
        decimal? creditCashAmount,
        string? internalNotes,
        string updatedBy)
    {
        AgencyCost = agencyCost;
        DesiredProfitAmount = desiredProfitAmount;
        DesiredProfitPercentage = desiredProfitPercentage;
        PixDiscountPercentage = pixDiscountPercentage;
        PixAmount = pixAmount;
        CreditCashAmount = creditCashAmount;
        InternalNotes = internalNotes;
        SetUpdated(updatedBy);
    }

    public void Rename(string name, string updatedBy)
    {
        Name = name;
        SetUpdated(updatedBy);
    }

    public void SetCalculatedAmounts(decimal? creditCashAmount, decimal? pixAmount, string updatedBy)
    {
        CreditCashAmount = creditCashAmount;
        PixAmount = pixAmount;
        SetUpdated(updatedBy);
    }

    public bool CanBeUpdated() => !IsDeleted;
}

