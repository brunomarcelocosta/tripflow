namespace Tripflow.Application.DTOs.Requests.Tenants;

public sealed record CreateTenantRequest(string LegalName,
                                         string TradeName,
                                         string? DocumentNumber,
                                         string? Email,
                                         string? Phone);