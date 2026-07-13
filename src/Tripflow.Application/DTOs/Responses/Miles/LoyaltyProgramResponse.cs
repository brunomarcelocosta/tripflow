using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Miles;

public sealed record LoyaltyProgramResponse(
    Guid Id,
    string Name,
    string? Country,
    string? AirlineName,
    LoyaltyProgramStatus Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
