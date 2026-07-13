namespace Tripflow.Application.DTOs.Requests.Proposals;

public sealed record UpdateProposalRequest(
    Guid? QuotePricingOptionId,
    DateTime? ExpiresAtUtc);
