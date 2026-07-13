using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Quotes;

public sealed class Itinerary : AuditableEntity, ITenantEntity
{
    private Itinerary() { }

    public Itinerary(Guid tenantId, Guid quoteId, string name, int? totalDays, int? totalNights, string createdBy)
    {
        TenantId = tenantId;
        QuoteId = quoteId;
        Name = name;
        TotalDays = totalDays;
        TotalNights = totalNights;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid QuoteId { get; private set; }
    public Quote Quote { get; private set; } = default!;

    public string Name { get; private set; } = default!;
    public int? TotalDays { get; private set; }
    public int? TotalNights { get; private set; }

    public List<ItineraryStop> Stops = [];

    public void Update(string name, int? totalDays, int? totalNights, string updatedBy)
    {
        Name = name;
        TotalDays = totalDays;
        TotalNights = totalNights;
        SetUpdated(updatedBy);
    }

    public void RecalculateTotalsFromStops(IEnumerable<ItineraryStop> stops, string updatedBy)
    {
        var totalNights = stops?.Sum(s => s.Nights) ?? 0;
        TotalNights = totalNights;
        TotalDays = totalNights > 0 ? totalNights + 1 : 0;
        SetUpdated(updatedBy);
    }
}

