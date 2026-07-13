using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Miles;

public sealed record CustomerLoyaltyAccountResponse(
    Guid Id,
    Guid TenantId,
    Guid CustomerId,
    string CustomerName,
    Guid LoyaltyProgramId,
    string LoyaltyProgramName,
    string? AccountNumber,
    int CurrentBalance,
    decimal? AverageCostPerThousand,
    DateTime? LastBalanceUpdateUtc,
    string? Notes,
    LoyaltyAccountStatus Status,
    int ExpiringMilesInNext90Days,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
