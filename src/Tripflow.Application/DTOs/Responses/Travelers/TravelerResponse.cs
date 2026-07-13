namespace Tripflow.Application.DTOs.Responses.Travelers;

public sealed record TravelerResponse(
    Guid Id,
    Guid TenantId,
    Guid CustomerId,
    string FullName,
    DateOnly? BirthDate,
    string? Nationality,
    string? DocumentNumber,
    string? PassportNumber,
    DateOnly? PassportExpirationDate,
    string? Notes,
    bool PassportExpired,
    bool PassportExpiringSoon,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
