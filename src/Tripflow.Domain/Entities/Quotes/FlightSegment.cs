using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Quotes;

public sealed class FlightSegment : AuditableEntity, ITenantEntity
{
    private FlightSegment() { }

    public FlightSegment(Guid tenantId, Guid quoteFlightItemId, string originAirport, string destinationAirport, string? originCity, string? destinationCity, DateTime? departureDateTimeUtc, DateTime? arrivalDateTimeUtc, string? flightNumber, string? airlineName, int sequence, string createdBy)
    {
        TenantId = tenantId;
        QuoteFlightItemId = quoteFlightItemId;
        OriginAirport = originAirport;
        DestinationAirport = destinationAirport;
        OriginCity = originCity;
        DestinationCity = destinationCity;
        DepartureDateTimeUtc = departureDateTimeUtc;
        ArrivalDateTimeUtc = arrivalDateTimeUtc;
        FlightNumber = flightNumber;
        AirlineName = airlineName;
        Sequence = sequence;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid QuoteFlightItemId { get; private set; }
    public QuoteFlightItem QuoteFlightItem { get; private set; } = default!;

    public string OriginAirport { get; private set; } = default!;
    public string DestinationAirport { get; private set; } = default!;
    public string? OriginCity { get; private set; }
    public string? DestinationCity { get; private set; }
    public DateTime? DepartureDateTimeUtc { get; private set; }
    public DateTime? ArrivalDateTimeUtc { get; private set; }
    public string? FlightNumber { get; private set; }
    public string? AirlineName { get; private set; }
    public int Sequence { get; private set; }

    public void Update(
        string originAirport,
        string destinationAirport,
        string? originCity,
        string? destinationCity,
        DateTime? departureDateTimeUtc,
        DateTime? arrivalDateTimeUtc,
        string? flightNumber,
        string? airlineName,
        int sequence,
        string updatedBy)
    {
        OriginAirport = originAirport;
        DestinationAirport = destinationAirport;
        OriginCity = originCity;
        DestinationCity = destinationCity;
        DepartureDateTimeUtc = departureDateTimeUtc;
        ArrivalDateTimeUtc = arrivalDateTimeUtc;
        FlightNumber = flightNumber;
        AirlineName = airlineName;
        Sequence = sequence;
        SetUpdated(updatedBy);
    }

    public void UpdateSequence(int sequence, string updatedBy)
    {
        Sequence = sequence;
        SetUpdated(updatedBy);
    }
}

