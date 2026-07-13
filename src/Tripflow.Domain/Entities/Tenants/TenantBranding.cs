using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Tenants;

public sealed class TenantBranding : AuditableEntity, ITenantEntity
{
    private TenantBranding() { }

    public TenantBranding(Guid tenantId, string? logoUrl, string? primaryColor, string? secondaryColor, string? textColor, string? proposalFooter, string createdBy)
    {
        TenantId = tenantId;
        LogoUrl = logoUrl;
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        TextColor = textColor;
        ProposalFooter = proposalFooter;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public string? LogoUrl { get; private set; }
    public string? PrimaryColor { get; private set; }
    public string? SecondaryColor { get; private set; }
    public string? TextColor { get; private set; }
    public string? ProposalFooter { get; private set; }

    public void UpdateColorsAndFooter(
        string? primaryColor,
        string? secondaryColor,
        string? textColor,
        string? proposalFooter,
        string updatedBy)
    {
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        TextColor = textColor;
        ProposalFooter = proposalFooter;
        SetUpdated(updatedBy);
    }

    public void UpdateLogo(string? logoUrl, string updatedBy)
    {
        LogoUrl = logoUrl;
        SetUpdated(updatedBy);
    }
}

