using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Proposals;

public sealed record ProposalEventResponse(
    Guid Id,
    Guid TenantId,
    Guid ProposalId,
    ProposalEventType EventType,
    string? Description,
    string? IpAddress,
    string? UserAgent,
    DateTime CreatedAtUtc);
