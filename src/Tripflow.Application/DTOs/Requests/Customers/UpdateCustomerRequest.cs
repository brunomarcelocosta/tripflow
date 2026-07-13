using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Customers;

public sealed record UpdateCustomerRequest(
    CustomerType Type,
    string FullName,
    string? DocumentNumber,
    string? Email,
    string? Phone,
    DateOnly? BirthDate,
    string? Notes,
    CustomerStatus Status);
