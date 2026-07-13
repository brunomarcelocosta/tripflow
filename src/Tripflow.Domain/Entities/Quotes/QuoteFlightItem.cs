using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Quotes;

public sealed class QuoteFlightItem : AuditableEntity, ITenantEntity
{
    private QuoteFlightItem() { }

    public QuoteFlightItem(Guid tenantId, Guid quoteId, string? airlineName, string? fareFamily, string? baggageDescription, bool includedPersonalItem, bool includedCarryOn, decimal? carryOnWeightKg, bool includedCheckedBag, decimal? checkedBagWeightKg, string createdBy)
    {
        TenantId = tenantId;
        QuoteId = quoteId;
        AirlineName = airlineName;
        FareFamily = fareFamily;
        BaggageDescription = baggageDescription;
        IncludedPersonalItem = includedPersonalItem;
        IncludedCarryOn = includedCarryOn;
        CarryOnWeightKg = carryOnWeightKg;
        IncludedCheckedBag = includedCheckedBag;
        CheckedBagWeightKg = checkedBagWeightKg;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid QuoteId { get; private set; }
    public Quote Quote { get; private set; } = default!;

    public string? AirlineName { get; private set; }
    public string? FareFamily { get; private set; }
    public string? BaggageDescription { get; private set; }
    public bool IncludedPersonalItem { get; private set; }
    public bool IncludedCarryOn { get; private set; }
    public decimal? CarryOnWeightKg { get; private set; }
    public bool IncludedCheckedBag { get; private set; }
    public decimal? CheckedBagWeightKg { get; private set; }

    public List<FlightSegment> Segments = [];

    public void Update(
        string? airlineName,
        string? fareFamily,
        string? baggageDescription,
        bool includedPersonalItem,
        bool includedCarryOn,
        decimal? carryOnWeightKg,
        bool includedCheckedBag,
        decimal? checkedBagWeightKg,
        string updatedBy)
    {
        AirlineName = airlineName;
        FareFamily = fareFamily;
        BaggageDescription = baggageDescription;
        IncludedPersonalItem = includedPersonalItem;
        IncludedCarryOn = includedCarryOn;
        CarryOnWeightKg = carryOnWeightKg;
        IncludedCheckedBag = includedCheckedBag;
        CheckedBagWeightKg = checkedBagWeightKg;
        SetUpdated(updatedBy);
    }

    public void UpdateBaggage(
        string? baggageDescription,
        bool includedPersonalItem,
        bool includedCarryOn,
        decimal? carryOnWeightKg,
        bool includedCheckedBag,
        decimal? checkedBagWeightKg,
        string updatedBy)
    {
        BaggageDescription = baggageDescription;
        IncludedPersonalItem = includedPersonalItem;
        IncludedCarryOn = includedCarryOn;
        CarryOnWeightKg = carryOnWeightKg;
        IncludedCheckedBag = includedCheckedBag;
        CheckedBagWeightKg = checkedBagWeightKg;
        SetUpdated(updatedBy);
    }
}

