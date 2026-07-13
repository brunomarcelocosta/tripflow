using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Payments;

public sealed record FinancialTransactionResponse(
    Guid Id,
    Guid TenantId,
    Guid? PaymentId,
    Guid? QuoteId,
    FinancialTransactionType Type,
    decimal GrossAmount,
    decimal? FeeAmount,
    decimal? NetAmount,
    decimal? AgencyCost,
    decimal? ProfitAmount,
    DateTime TransactionDateUtc,
    string? Description);
