namespace Tripflow.Application.DTOs.Responses.Tenants;

public sealed class UploadTenantLogoResponse
{
    public Guid TenantId { get; init; }
    public string LogoUrl { get; init; } = default!;
}
