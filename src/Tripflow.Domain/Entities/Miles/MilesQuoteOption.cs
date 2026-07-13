using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Miles;

public sealed class MilesQuoteOption : AuditableEntity, ITenantEntity
{
    private MilesQuoteOption() { }

    public MilesQuoteOption(
        Guid tenantId,
        Guid quoteId,
        Guid? loyaltyProgramId,
        string name,
        int milesAmount,
        decimal? boardingFees,
        decimal? costPerThousand,
        decimal? equivalentCost,
        decimal? cashPrice,
        decimal? estimatedSavings,
        decimal? serviceFee,
        decimal? totalAmount,
        bool selected,
        string? notes,
        string createdBy)
    {
        TenantId = tenantId;
        QuoteId = quoteId;
        LoyaltyProgramId = loyaltyProgramId;
        Name = name;
        MilesAmount = milesAmount;
        BoardingFees = boardingFees;
        CostPerThousand = costPerThousand;
        EquivalentCost = equivalentCost;
        CashPrice = cashPrice;
        EstimatedSavings = estimatedSavings;
        ServiceFee = serviceFee;
        TotalAmount = totalAmount;
        Selected = selected;
        Notes = notes;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid QuoteId { get; private set; }
    public Quote Quote { get; private set; } = default!;

    public Guid? LoyaltyProgramId { get; private set; }
    public LoyaltyProgram? LoyaltyProgram { get; private set; }

    public string Name { get; private set; } = default!;

    public int MilesAmount { get; private set; }
    public decimal? BoardingFees { get; private set; }
    public decimal? CostPerThousand { get; private set; }

    /// Example: 80,000 miles at R$ 20.00 per thousand = R$ 1,600.00.
    /// </summary>
    public decimal? EquivalentCost { get; private set; }

    /// <summary>
    /// Cash price used as comparison against the miles option.
    /// </summary>
    public decimal? CashPrice { get; private set; }

    public decimal? EstimatedSavings { get; private set; }

    /// <summary>
    /// Agency service fee charged for mileage issuance or advisory.
    /// </summary>
    public decimal? ServiceFee { get; private set; }

    /// <summary>
    /// Final amount to be charged to the customer for this miles option.
    /// Usually boarding fees + service fee + any additional manual cost.
    /// </summary>
    public decimal? TotalAmount { get; private set; }

    public bool Selected { get; private set; }
    public string? Notes { get; private set; }

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

    public void Update(
        string name,
        Guid? loyaltyProgramId,
        int milesAmount,
        decimal? boardingFees,
        decimal? costPerThousand,
        decimal? equivalentCost,
        decimal? cashPrice,
        decimal? estimatedSavings,
        decimal? serviceFee,
        decimal? totalAmount,
        string? notes,
        string updatedBy)
    {
        Name = name;
        LoyaltyProgramId = loyaltyProgramId;
        MilesAmount = milesAmount;
        BoardingFees = boardingFees;
        CostPerThousand = costPerThousand;
        EquivalentCost = equivalentCost;
        CashPrice = cashPrice;
        EstimatedSavings = estimatedSavings;
        ServiceFee = serviceFee;
        TotalAmount = totalAmount;
        Notes = notes;
        SetUpdated(updatedBy);
    }

    public void RecalculateAmounts(string updatedBy)
    {
        var equivalent = (CostPerThousand.HasValue && MilesAmount > 0)
            ? Math.Round((decimal)MilesAmount / 1000m * CostPerThousand.Value, 2)
            : (decimal?)null;

        EquivalentCost = equivalent;

        var boarding = BoardingFees ?? 0m;
        var service = ServiceFee ?? 0m;
        var equiv = equivalent ?? 0m;
        TotalAmount = Math.Round(boarding + service + equiv, 2);

        EstimatedSavings = CashPrice.HasValue
            ? Math.Round(CashPrice.Value - TotalAmount.Value, 2)
            : (decimal?)null;

        SetUpdated(updatedBy);
    }
}

