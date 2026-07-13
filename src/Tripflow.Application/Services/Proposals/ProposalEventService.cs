using Tripflow.Domain.Entities.Proposals;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.Services.Proposals;

public class ProposalEventService(IProposalEventRepository eventRepository) : IProposalEventService
{
    public async Task RegisterAsync(
        Guid tenantId,
        Guid proposalId,
        ProposalEventType eventType,
        string? description = null,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        var evt = new ProposalEvent(tenantId, proposalId, eventType, description, ipAddress, userAgent);
        await eventRepository.AddAsync(evt, cancellationToken);
    }
}
