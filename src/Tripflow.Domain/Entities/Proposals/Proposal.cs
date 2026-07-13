using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Proposals;

public sealed class Proposal : AuditableEntity, ITenantEntity
{
    private Proposal() { }

    public Proposal(Guid tenantId, Guid quoteId, Guid? quotePricingOptionId, string proposalNumber, ProposalStatus status, string? publicToken, string? publicUrl, string? pdfUrl, DateTime? expiresAtUtc, string createdBy)
    {
        TenantId = tenantId;
        QuoteId = quoteId;
        QuotePricingOptionId = quotePricingOptionId;
        ProposalNumber = proposalNumber;
        Status = status;
        PublicToken = publicToken;
        PublicUrl = publicUrl;
        PdfUrl = pdfUrl;
        ExpiresAtUtc = expiresAtUtc;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid QuoteId { get; private set; }
    public Quote Quote { get; private set; } = default!;

    public Guid? QuotePricingOptionId { get; private set; }
    public QuotePricingOption? QuotePricingOption { get; private set; }

    public string ProposalNumber { get; private set; } = default!;
    public ProposalStatus Status { get; private set; }
    public string? PublicToken { get; private set; }
    public string? PublicUrl { get; private set; }
    public string? PdfUrl { get; private set; }
    public DateTime? ViewedAtUtc { get; private set; }
    public DateTime? ApprovedAtUtc { get; private set; }
    public DateTime? RejectedAtUtc { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }

    public List<ProposalVersion> Versions = [];
    public List<ProposalEvent> Events = [];
    public List<Payment> Payments = [];

    public void Update(Guid? quotePricingOptionId, DateTime? expiresAtUtc, string updatedBy)
    {
        QuotePricingOptionId = quotePricingOptionId;
        ExpiresAtUtc = expiresAtUtc;
        SetUpdated(updatedBy);
    }

    public void SetPublicLink(string publicToken, string publicUrl, string updatedBy)
    {
        PublicToken = publicToken;
        PublicUrl = publicUrl;
        SetUpdated(updatedBy);
    }

    public void SetPdfUrl(string? pdfUrl, string updatedBy)
    {
        PdfUrl = pdfUrl;
        SetUpdated(updatedBy);
    }

    public void MarkAsGenerated(string updatedBy)
    {
        Status = ProposalStatus.Generated;
        SetUpdated(updatedBy);
    }

    public void MarkAsSent(string updatedBy)
    {
        Status = ProposalStatus.Sent;
        SetUpdated(updatedBy);
    }

    public void MarkAsViewed(string updatedBy)
    {
        ViewedAtUtc ??= DateTime.UtcNow;
        if (Status is ProposalStatus.Sent or ProposalStatus.Generated)
            Status = ProposalStatus.Viewed;
        SetUpdated(updatedBy);
    }

    public void Approve(string updatedBy)
    {
        Status = ProposalStatus.Approved;
        ApprovedAtUtc = DateTime.UtcNow;
        SetUpdated(updatedBy);
    }

    public void Reject(string updatedBy)
    {
        Status = ProposalStatus.Rejected;
        RejectedAtUtc = DateTime.UtcNow;
        SetUpdated(updatedBy);
    }

    public void Expire(string updatedBy)
    {
        Status = ProposalStatus.Expired;
        SetUpdated(updatedBy);
    }

    public void Cancel(string updatedBy)
    {
        Status = ProposalStatus.Cancelled;
        SetUpdated(updatedBy);
    }

    public bool CanBeUpdated()
        => Status is not (ProposalStatus.Approved or ProposalStatus.Cancelled or ProposalStatus.Expired);

    public bool CanBeCancelled()
        => Status is not ProposalStatus.Approved;

    public bool IsExpired(DateTime utcNow)
        => ExpiresAtUtc.HasValue && ExpiresAtUtc.Value < utcNow;
}

