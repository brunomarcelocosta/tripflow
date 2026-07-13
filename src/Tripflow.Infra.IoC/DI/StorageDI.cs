using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tripflow.Domain.Interfaces.Storage;
using Tripflow.Infra.Storage;

namespace Tripflow.Infra.IoC.DI;

public static class StorageDI
{
    public static IServiceCollection AddStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<StorageOptions>()
            .Bind(configuration.GetSection(StorageOptions.SectionName))
            .Validate(x => !string.IsNullOrWhiteSpace(x.Provider), "Storage provider is required.")
            .ValidateOnStart();

        var provider = configuration[$"{StorageOptions.SectionName}:Provider"];

        if (string.Equals(provider, "AzureBlob", StringComparison.OrdinalIgnoreCase))
        {
            services.AddScoped<IFileStorageService, AzureBlobStorageService>();
        }
        else
        {
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
        }

        return services;
    }
}
