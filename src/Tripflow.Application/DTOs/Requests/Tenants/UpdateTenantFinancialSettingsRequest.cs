namespace Tripflow.Application.DTOs.Requests.Tenants;

public sealed record UpdateTenantFinancialSettingsRequest
{
    public decimal? DefaultProfitAmount { get; init; }
    public decimal? DefaultProfitPercentage { get; init; }
    public decimal? DefaultPixDiscountPercentage { get; init; }
}
