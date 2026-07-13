using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Tenants;

public sealed record UpdateTenantRequest(
    string LegalName,
    string TradeName,
    string? DocumentNumber,
    string? Email,
    string? Phone,
    TenantStatus Status);