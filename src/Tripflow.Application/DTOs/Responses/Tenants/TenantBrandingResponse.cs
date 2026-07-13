namespace Tripflow.Application.DTOs.Responses.Tenants;

public sealed class TenantBrandingResponse
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public string? LogoUrl { get; init; }
    public string? PrimaryColor { get; init; }
    public string? SecondaryColor { get; init; }
    public string? TextColor { get; init; }
    public string? ProposalFooter { get; init; }
    public DateTime? CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
