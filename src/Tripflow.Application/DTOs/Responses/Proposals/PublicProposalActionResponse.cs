using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Proposals;

public sealed record PublicProposalActionResponse(
    Guid ProposalId,
    ProposalStatus Status,
    DateTime? ApprovedAtUtc,
    DateTime? RejectedAtUtc);
