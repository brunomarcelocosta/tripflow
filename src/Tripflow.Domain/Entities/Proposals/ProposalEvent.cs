using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Proposals;

public sealed class ProposalEvent : BaseEntity, ITenantEntity
{
    private ProposalEvent() { }

    public ProposalEvent(Guid tenantId, Guid proposalId, ProposalEventType eventType, string? description, string? ipAddress, string? userAgent)
    {
        TenantId = tenantId;
        ProposalId = proposalId;
        EventType = eventType;
        Description = description;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid ProposalId { get; private set; }
    public Proposal Proposal { get; private set; } = default!;

    public ProposalEventType EventType { get; private set; }
    public string? Description { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}

