namespace Tripflow.Application.DTOs.Requests.Tenants;

public sealed record UploadTenantLogoRequest
{
    public Stream Content { get; init; } = Stream.Null;
    public string FileName { get; init; } = default!;
    public string ContentType { get; init; } = default!;
    public long SizeBytes { get; init; }
}
