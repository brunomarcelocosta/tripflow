using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Payments;

public sealed class PaymentFilterRequest : FilterRequest
{
    public Guid? ProposalId { get; set; }
    public Guid? QuoteId { get; set; }
    public PaymentStatus? Status { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public DateOnly? DueDateFrom { get; set; }
    public DateOnly? DueDateTo { get; set; }
    public DateTime? PaidFromUtc { get; set; }
    public DateTime? PaidToUtc { get; set; }
}
