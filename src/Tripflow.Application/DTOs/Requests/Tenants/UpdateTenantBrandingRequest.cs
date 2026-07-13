namespace Tripflow.Application.DTOs.Requests.Tenants;

public sealed record UpdateTenantBrandingRequest
{
    public string? PrimaryColor { get; init; }
    public string? SecondaryColor { get; init; }
    public string? TextColor { get; init; }
    public string? ProposalFooter { get; init; }
}
