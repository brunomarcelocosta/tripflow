using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Miles;

public sealed class CustomerLoyaltyAccountFilterRequest : FilterRequest
{
    public Guid? LoyaltyProgramId { get; set; }
    public LoyaltyAccountStatus? Status { get; set; }
    public string? AccountNumber { get; set; }
}

public sealed record CreateCustomerLoyaltyAccountRequest(
    Guid LoyaltyProgramId,
    string? AccountNumber,
    int CurrentBalance,
    decimal? AverageCostPerThousand,
    string? Notes,
    LoyaltyAccountStatus? Status);

public sealed record UpdateCustomerLoyaltyAccountRequest(
    Guid LoyaltyProgramId,
    string? AccountNumber,
    int CurrentBalance,
    decimal? AverageCostPerThousand,
    string? Notes,
    LoyaltyAccountStatus Status);
