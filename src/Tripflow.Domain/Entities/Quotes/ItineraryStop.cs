using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities;

public sealed class ItineraryStop : AuditableEntity, ITenantEntity
{
    private ItineraryStop() { }

    public ItineraryStop(Guid tenantId, Guid itineraryId, string country, string city, int nights, int sequence, string? notes, string createdBy)
    {
        TenantId = tenantId;
        ItineraryId = itineraryId;
        Country = country;
        City = city;
        Nights = nights;
        Sequence = sequence;
        Notes = notes;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid ItineraryId { get; private set; }
    public Itinerary Itinerary { get; private set; } = default!;

    public string Country { get; private set; } = default!;
    public string City { get; private set; } = default!;
    public int Nights { get; private set; }
    public int Sequence { get; private set; }
    public string? Notes { get; private set; }

    public void Update(string country, string city, int nights, int sequence, string? notes, string updatedBy)
    {
        Country = country;
        City = city;
        Nights = nights;
        Sequence = sequence;
        Notes = notes;
        SetUpdated(updatedBy);
    }

    public void UpdateSequence(int sequence, string updatedBy)
    {
        Sequence = sequence;
        SetUpdated(updatedBy);
    }
}

