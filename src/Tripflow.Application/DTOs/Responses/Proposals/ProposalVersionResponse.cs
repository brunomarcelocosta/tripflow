namespace Tripflow.Application.DTOs.Responses.Proposals;

public sealed record ProposalVersionResponse(
    Guid Id,
    Guid TenantId,
    Guid ProposalId,
    int VersionNumber,
    string? PdfUrl,
    Guid? GeneratedByUserId,
    DateTime CreatedAtUtc);
