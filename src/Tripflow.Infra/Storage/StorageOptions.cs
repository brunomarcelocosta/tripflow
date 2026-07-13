namespace Tripflow.Infra.Storage;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public string Provider { get; set; } = "Local";

    public string? ConnectionString { get; set; }

    public string ContainerName { get; set; } = "tenant-assets";

    public string LocalBasePath { get; set; } = "wwwroot/uploads";

    public string? PublicBaseUrl { get; set; }

    public bool UsePublicAccess { get; set; } = true;
}
