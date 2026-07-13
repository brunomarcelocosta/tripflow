using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Proposals;

public sealed class ProposalFilterRequest : FilterRequest
{
    public Guid? QuoteId { get; set; }
    public ProposalStatus? Status { get; set; }
    public string? ProposalNumber { get; set; }
    public DateTime? CreatedFromUtc { get; set; }
    public DateTime? CreatedToUtc { get; set; }
    public DateTime? ExpiresFromUtc { get; set; }
    public DateTime? ExpiresToUtc { get; set; }
}
