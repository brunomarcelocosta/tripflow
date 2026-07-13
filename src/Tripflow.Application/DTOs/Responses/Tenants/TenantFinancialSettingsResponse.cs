namespace Tripflow.Application.DTOs.Responses.Tenants;

public sealed class TenantFinancialSettingsResponse
{
    public Guid TenantId { get; init; }
    public decimal? DefaultProfitAmount { get; init; }
    public decimal? DefaultProfitPercentage { get; init; }
    public decimal? DefaultPixDiscountPercentage { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
