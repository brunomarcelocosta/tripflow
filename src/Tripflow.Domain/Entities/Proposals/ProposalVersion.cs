using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Proposals;

public sealed class ProposalVersion : AuditableEntity, ITenantEntity
{
    private ProposalVersion() { }

    public ProposalVersion(Guid tenantId, Guid proposalId, int versionNumber, string? htmlSnapshot, string? pdfUrl, Guid? generatedByUserId, string createdBy)
    {
        TenantId = tenantId;
        ProposalId = proposalId;
        VersionNumber = versionNumber;
        HtmlSnapshot = htmlSnapshot;
        PdfUrl = pdfUrl;
        GeneratedByUserId = generatedByUserId;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid ProposalId { get; private set; }
    public Proposal Proposal { get; private set; } = default!;

    public int VersionNumber { get; private set; }
    public string? HtmlSnapshot { get; private set; }
    public string? PdfUrl { get; private set; }
    public Guid? GeneratedByUserId { get; private set; }
    public UserProfile? GeneratedByUser { get; private set; }

    public void SetPdfUrl(string? pdfUrl, string updatedBy)
    {
        PdfUrl = pdfUrl;
        SetUpdated(updatedBy);
    }
}

