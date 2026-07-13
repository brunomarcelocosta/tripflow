namespace Tripflow.Application.DTOs.Requests.Proposals;

public sealed record CreateProposalFromQuoteRequest(
    Guid? QuotePricingOptionId,
    DateTime? ExpiresAtUtc,
    bool GeneratePdf,
    bool GeneratePublicLink);
