using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tripflow.Domain.Interfaces.Storage;

namespace Tripflow.Infra.Storage;

public sealed class AzureBlobStorageService(
    IOptions<StorageOptions> options,
    ILogger<AzureBlobStorageService> logger) : IFileStorageService
{
    private readonly StorageOptions _options = options.Value;

    public async Task<string> UploadAsync(
        Stream fileStream,
        string folder,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required.", nameof(fileName));

        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
            throw new InvalidOperationException("Azure Blob Storage connection string is not configured.");

        if (string.IsNullOrWhiteSpace(_options.ContainerName))
            throw new InvalidOperationException("Azure Blob Storage container name is not configured.");

        var blobServiceClient = new BlobServiceClient(_options.ConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_options.ContainerName);

        await containerClient.CreateIfNotExistsAsync(
            _options.UsePublicAccess ? PublicAccessType.Blob : PublicAccessType.None,
            cancellationToken: cancellationToken);

        var safeFileName = BuildSafeFileName(fileName);
        var normalizedFolder = NormalizeFolder(folder);

        var blobName = string.IsNullOrEmpty(normalizedFolder)
            ? safeFileName
            : $"{normalizedFolder}/{safeFileName}";

        var blobClient = containerClient.GetBlobClient(blobName);

        if (fileStream.CanSeek)
            fileStream.Position = 0;

        await blobClient.UploadAsync(
            fileStream,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            },
            cancellationToken: cancellationToken);

        logger.LogInformation(
            "File uploaded to Azure Blob Storage. Container: {Container}. BlobName: {BlobName}",
            _options.ContainerName,
            blobName);

        return blobClient.Uri.ToString();
    }

    public async Task DeleteAsync(string fileUrlOrPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrlOrPath))
            return;

        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
            return;

        try
        {
            var blobServiceClient = new BlobServiceClient(_options.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_options.ContainerName);

            var blobName = ExtractBlobName(fileUrlOrPath);

            if (string.IsNullOrWhiteSpace(blobName))
                return;

            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync(
                DeleteSnapshotsOption.IncludeSnapshots,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error deleting file from Azure Blob Storage. File: {File}", fileUrlOrPath);
        }
    }

    private static string BuildSafeFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

        var safeName = string.Join(
            "-",
            nameWithoutExtension
                .Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries))
            .Trim()
            .ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(safeName))
            safeName = "file";

        return $"{Guid.NewGuid():N}-{safeName}{extension.ToLowerInvariant()}";
    }

    private static string NormalizeFolder(string folder)
    {
        if (string.IsNullOrWhiteSpace(folder))
            return string.Empty;

        return folder
            .Replace('\\', '/')
            .Trim()
            .Trim('/');
    }

    private string? ExtractBlobName(string fileUrlOrPath)
    {
        if (Uri.TryCreate(fileUrlOrPath, UriKind.Absolute, out var uri))
        {
            var path = uri.AbsolutePath.TrimStart('/');

            var containerPrefix = $"{_options.ContainerName}/";

            if (path.StartsWith(containerPrefix, StringComparison.OrdinalIgnoreCase))
                return path[containerPrefix.Length..];

            return path;
        }

        return fileUrlOrPath.Replace('\\', '/').TrimStart('/');
    }
}
