namespace Tripflow.Application.DTOs.Requests.Payments;

public class FinancialTransactionFilterRequest : FilterRequest
{
    public Guid? PaymentId { get; set; }
    public Guid? QuoteId { get; set; }
    public Domain.Enums.FinancialTransactionType? Type { get; set; }
    public DateTime? TransactionFromUtc { get; set; }
    public DateTime? TransactionToUtc { get; set; }
}
