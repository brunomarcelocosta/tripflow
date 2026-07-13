using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Quotes;

public sealed class QuoteItem : AuditableEntity, ITenantEntity
{
    private QuoteItem() { }

    public QuoteItem(Guid tenantId, Guid quoteId, QuoteItemType type, string title, string? description, decimal agencyCost, decimal saleAmount, string createdBy)
    {
        TenantId = tenantId;
        QuoteId = quoteId;
        Type = type;
        Title = title;
        Description = description;
        AgencyCost = agencyCost;
        SaleAmount = saleAmount;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid QuoteId { get; private set; }
    public Quote Quote { get; private set; } = default!;

    public QuoteItemType Type { get; private set; }
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal AgencyCost { get; private set; }
    public decimal SaleAmount { get; private set; }
    public string? Notes { get; private set; }

    public void Update(
        QuoteItemType type,
        string title,
        string? description,
        decimal agencyCost,
        decimal saleAmount,
        string? notes,
        string updatedBy)
    {
        Type = type;
        Title = title;
        Description = description;
        AgencyCost = agencyCost;
        SaleAmount = saleAmount;
        Notes = notes;
        SetUpdated(updatedBy);
    }

    public decimal GetEstimatedProfitAmount()
        => SaleAmount - AgencyCost;
}

