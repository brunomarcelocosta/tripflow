using Tripflow.Domain.Enums;

namespace Tripflow.Application.Services.Proposals;

public interface IProposalEventService
{
    Task RegisterAsync(
        Guid tenantId,
        Guid proposalId,
        ProposalEventType eventType,
        string? description = null,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);
}
