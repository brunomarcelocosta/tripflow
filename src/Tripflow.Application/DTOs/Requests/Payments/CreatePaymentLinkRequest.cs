namespace Tripflow.Application.DTOs.Requests.Payments;

public sealed record CreatePaymentLinkRequest(DateTime? ExpiresAtUtc);
