using Tripflow.Domain.Entities.Tenants;

namespace Tripflow.UnitTests.Utils;

public static class TenantBrandingTestHelper
{
    public static TenantBranding Create(
        Guid? tenantId = null,
        string? logoUrl = "https://cdn.tripflow.test/logo.png",
        string? primaryColor = "#000000",
        string? secondaryColor = "#FFFFFF",
        string? textColor = "#1A1A1A",
        string? proposalFooter = "Rodapé padrão.",
        string createdBy = "system")
    {
        return new TenantBranding(
            tenantId ?? Guid.NewGuid(),
            logoUrl,
            primaryColor,
            secondaryColor,
            textColor,
            proposalFooter,
            createdBy);
    }
}

public static class TenantCommercialSettingsTestHelper
{
    public static TenantCommercialSettings Create(
        Guid? tenantId = null,
        string createdBy = "system")
    {
        return new TenantCommercialSettings(
            tenantId ?? Guid.NewGuid(),
            createdBy);
    }

    public static TenantCommercialSettings CreateWithCommercialData(
        Guid? tenantId = null,
        string? commercialEmail = "comercial@empresa.com",
        string? commercialPhone = "11999990000",
        string? whatsApp = "11988887777",
        string? instagram = "@empresa",
        string? website = "https://empresa.com",
        string? defaultTerms = "Termos padrão.",
        string? defaultImportantNotes = "Notas importantes.",
        int defaultProposalExpirationHours = 48,
        string createdBy = "system")
    {
        var entity = new TenantCommercialSettings(tenantId ?? Guid.NewGuid(), createdBy);

        entity.UpdateCommercialData(
            commercialEmail,
            commercialPhone,
            whatsApp,
            instagram,
            website,
            defaultTerms,
            defaultImportantNotes,
            defaultProposalExpirationHours,
            createdBy);

        return entity;
    }

    public static TenantCommercialSettings CreateWithFinancialData(
        Guid? tenantId = null,
        decimal? defaultProfitAmount = 100m,
        decimal? defaultProfitPercentage = null,
        decimal? defaultPixDiscountPercentage = 5m,
        string createdBy = "system")
    {
        var entity = new TenantCommercialSettings(tenantId ?? Guid.NewGuid(), createdBy);

        entity.UpdateFinancialDefaults(
            defaultProfitAmount,
            defaultProfitPercentage,
            defaultPixDiscountPercentage,
            createdBy);

        return entity;
    }
}
