using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Entities.Proposals;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Quotes;

public sealed class Quote : AuditableEntity, ITenantEntity
{
    private Quote() { }

    public Quote(Guid tenantId, Guid? customerId, string quoteNumber, string title, QuoteType type, string? origin, string? destination, DateOnly? departureDate, DateOnly? returnDate, int adults, int children, int infants, string? notes, DateTime? expiresAtUtc, Guid? createdByUserId, string createdBy)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        QuoteNumber = quoteNumber;
        Title = title;
        Type = type;
        Status = QuoteStatus.Draft;
        Origin = origin;
        Destination = destination;
        DepartureDate = departureDate;
        ReturnDate = returnDate;
        Adults = adults;
        Children = children;
        Infants = infants;
        Notes = notes;
        ExpiresAtUtc = expiresAtUtc;
        CreatedByUserId = createdByUserId;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid? CustomerId { get; private set; }
    public Customer? Customer { get; private set; }

    public string QuoteNumber { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public QuoteType Type { get; private set; }
    public QuoteStatus Status { get; private set; }
    public string? Origin { get; private set; }
    public string? Destination { get; private set; }
    public DateOnly? DepartureDate { get; private set; }
    public DateOnly? ReturnDate { get; private set; }
    public int Adults { get; private set; }
    public int Children { get; private set; }
    public int Infants { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }

    public Guid? CreatedByUserId { get; private set; }
    public UserProfile? CreatedByUser { get; private set; }

    public Itinerary? Itinerary { get; private set; }
    public List<QuoteItem> Items = [];
    public List<QuoteFlightItem> FlightItems = [];
    public List<QuotePricingOption> PricingOptions = [];
    public List<MilesQuoteOption> MilesQuoteOptions = [];
    public List<Proposal> Proposals = [];

    public void Update(
        Guid? customerId,
        string title,
        QuoteType type,
        string? origin,
        string? destination,
        DateOnly? departureDate,
        DateOnly? returnDate,
        int adults,
        int children,
        int infants,
        string? notes,
        DateTime? expiresAtUtc,
        string updatedBy)
    {
        CustomerId = customerId;
        Title = title;
        Type = type;
        Origin = origin;
        Destination = destination;
        DepartureDate = departureDate;
        ReturnDate = returnDate;
        Adults = adults;
        Children = children;
        Infants = infants;
        Notes = notes;
        ExpiresAtUtc = expiresAtUtc;
        SetUpdated(updatedBy);
    }

    public void MarkAsCalculated(string updatedBy)
    {
        Status = QuoteStatus.Calculated;
        SetUpdated(updatedBy);
    }

    public void Approve(string updatedBy)
    {
        Status = QuoteStatus.Approved;
        SetUpdated(updatedBy);
    }

    public void Reject(string updatedBy)
    {
        Status = QuoteStatus.Rejected;
        SetUpdated(updatedBy);
    }

    public void MarkAsPaid(string updatedBy)
    {
        Status = QuoteStatus.Paid;
        SetUpdated(updatedBy);
    }

    public void Cancel(string updatedBy)
    {
        Status = QuoteStatus.Cancelled;
        SetUpdated(updatedBy);
    }

    public bool CanBeCancelled()
        => Status is not (QuoteStatus.Paid or QuoteStatus.Issued or QuoteStatus.Cancelled);

    public bool CanBeUpdated()
        => Status is not (QuoteStatus.Cancelled or QuoteStatus.Paid or QuoteStatus.Issued);
}

