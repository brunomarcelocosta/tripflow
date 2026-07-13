using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Payments;

public sealed record CreatePaymentFromProposalRequest(
    Guid? TenantPaymentProviderId,
    PaymentMethod PaymentMethod,
    int Installments,
    decimal GrossAmount,
    decimal? FeeAmount,
    decimal? NetAmount,
    DateOnly? DueDate,
    bool CreatePaymentLink);
