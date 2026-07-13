using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Payments;

public sealed record PaymentProviderResponse(
    Guid Id,
    string Code,
    string Name,
    PaymentProviderStatus Status);
