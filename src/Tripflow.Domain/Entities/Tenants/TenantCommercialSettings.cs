using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Tenants;

public sealed class TenantCommercialSettings : AuditableEntity, ITenantEntity
{
    private TenantCommercialSettings() { }

    public TenantCommercialSettings(Guid tenantId, string createdBy)
    {
        TenantId = tenantId;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public string? CommercialEmail { get; private set; }
    public string? CommercialPhone { get; private set; }
    public string? WhatsApp { get; private set; }
    public string? Instagram { get; private set; }
    public string? Website { get; private set; }
    public decimal? DefaultProfitAmount { get; private set; }
    public decimal? DefaultProfitPercentage { get; private set; }
    public decimal? DefaultPixDiscountPercentage { get; private set; }
    public int DefaultProposalExpirationHours { get; private set; } = 24;
    public string? DefaultTerms { get; private set; }
    public string? DefaultImportantNotes { get; private set; }

    public void UpdateCommercialData(
        string? commercialEmail,
        string? commercialPhone,
        string? whatsApp,
        string? instagram,
        string? website,
        string? defaultTerms,
        string? defaultImportantNotes,
        int defaultProposalExpirationHours,
        string updatedBy)
    {
        CommercialEmail = commercialEmail;
        CommercialPhone = commercialPhone;
        WhatsApp = whatsApp;
        Instagram = instagram;
        Website = website;
        DefaultTerms = defaultTerms;
        DefaultImportantNotes = defaultImportantNotes;
        DefaultProposalExpirationHours = defaultProposalExpirationHours;
        SetUpdated(updatedBy);
    }

    public void UpdateFinancialDefaults(
        decimal? defaultProfitAmount,
        decimal? defaultProfitPercentage,
        decimal? defaultPixDiscountPercentage,
        string updatedBy)
    {
        DefaultProfitAmount = defaultProfitAmount;
        DefaultProfitPercentage = defaultProfitPercentage;
        DefaultPixDiscountPercentage = defaultPixDiscountPercentage;
        SetUpdated(updatedBy);
    }
}

