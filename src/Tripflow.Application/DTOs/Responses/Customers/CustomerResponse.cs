using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Customers;

public sealed record CustomerResponse(
    Guid Id,
    Guid TenantId,
    CustomerType Type,
    string FullName,
    string? DocumentNumber,
    string? Email,
    string? Phone,
    DateOnly? BirthDate,
    string? Notes,
    CustomerStatus Status,
    int TravelersCount,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
