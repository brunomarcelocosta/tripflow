using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Miles;

public sealed class LoyaltyProgramFilterRequest : FilterRequest
{
    public string? Country { get; set; }
    public string? AirlineName { get; set; }
    public LoyaltyProgramStatus? Status { get; set; }
}

public sealed record CreateLoyaltyProgramRequest(
    string Name,
    string? Country,
    string? AirlineName,
    LoyaltyProgramStatus? Status);

public sealed record UpdateLoyaltyProgramRequest(
    string Name,
    string? Country,
    string? AirlineName,
    LoyaltyProgramStatus Status);
