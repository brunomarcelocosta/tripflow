using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Customers;

public sealed record CreateCustomerRequest(
    CustomerType Type,
    string FullName,
    string? DocumentNumber,
    string? Email,
    string? Phone,
    DateOnly? BirthDate,
    string? Notes);
