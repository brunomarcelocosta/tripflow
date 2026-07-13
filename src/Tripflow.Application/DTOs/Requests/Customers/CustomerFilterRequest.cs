using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Customers;

public sealed class CustomerFilterRequest : FilterRequest
{
    public CustomerType? Type { get; set; }
    public CustomerStatus? Status { get; set; }
    public string? FullName { get; set; }
    public string? DocumentNumber { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
