using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Tenants;

public sealed class TenantFilterRequest : FilterRequest
{
    public string? LegalName { get; set; }
    public string? TradeName { get; set; }
    public string? DocumentNumber { get; set; }
    public string? Email { get; set; }
    public TenantStatus? Status { get; set; }
}