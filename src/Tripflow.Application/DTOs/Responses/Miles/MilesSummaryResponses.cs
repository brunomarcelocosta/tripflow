using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Miles;

public sealed record CustomerLoyaltyAccountSummaryResponse(
    Guid AccountId,
    Guid LoyaltyProgramId,
    string LoyaltyProgramName,
    string? AccountNumber,
    int CurrentBalance,
    int ExpiringIn30Days,
    int ExpiringIn60Days,
    int ExpiringIn90Days,
    decimal? AverageCostPerThousand,
    LoyaltyAccountStatus Status);

public sealed record CustomerMilesSummaryResponse(
    Guid CustomerId,
    string CustomerName,
    int TotalBalance,
    int TotalExpiringIn30Days,
    int TotalExpiringIn60Days,
    int TotalExpiringIn90Days,
    decimal? AverageCostPerThousand,
    IReadOnlyCollection<CustomerLoyaltyAccountSummaryResponse> Accounts);
