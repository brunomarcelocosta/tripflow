using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Tenants;

public sealed record UpdateTenantValidationRequest(
    Guid Id,
    string LegalName,
    string TradeName,
    string? DocumentNumber,
    string? Email,
    string? Phone,
    TenantStatus Status)
{
    public static UpdateTenantValidationRequest From(Guid id, UpdateTenantRequest request) =>
        new(id, request.LegalName, request.TradeName, request.DocumentNumber, request.Email, request.Phone, request.Status);
}
