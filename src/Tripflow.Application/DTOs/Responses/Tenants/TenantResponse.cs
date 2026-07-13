using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Tenants;

public sealed record TenantResponse(Guid Id,
                                    string LegalName,
                                    string TradeName,
                                    string? DocumentNumber,
                                    string? Email,
                                    string? Phone,
                                    TenantStatus Status,
                                    DateTime CreatedAtUtc,
                                    DateTime? UpdatedAtUtc);
