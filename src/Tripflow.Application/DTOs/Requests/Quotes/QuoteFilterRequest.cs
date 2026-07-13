using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Quotes;

public sealed class QuoteFilterRequest : FilterRequest
{
    public Guid? CustomerId { get; set; }
    public QuoteType? Type { get; set; }
    public QuoteStatus? Status { get; set; }
    public string? QuoteNumber { get; set; }
    public string? Title { get; set; }
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public DateOnly? DepartureFrom { get; set; }
    public DateOnly? DepartureTo { get; set; }
    public DateOnly? ReturnFrom { get; set; }
    public DateOnly? ReturnTo { get; set; }
}
