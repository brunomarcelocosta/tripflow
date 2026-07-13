using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tripflow.Domain.Interfaces.Storage;

namespace Tripflow.Infra.Storage;

public sealed class LocalFileStorageService(
    IOptions<StorageOptions> options,
    ILogger<LocalFileStorageService> logger) : IFileStorageService
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

        var basePath = ResolveBasePath();
        var normalizedFolder = NormalizeFolder(folder);

        var targetFolder = string.IsNullOrEmpty(normalizedFolder)
            ? basePath
            : Path.Combine(basePath, normalizedFolder.Replace('/', Path.DirectorySeparatorChar));

        Directory.CreateDirectory(targetFolder);

        var safeFileName = BuildSafeFileName(fileName);
        var fullPath = Path.Combine(targetFolder, safeFileName);

        if (fileStream.CanSeek)
            fileStream.Position = 0;

        await using (var output = File.Create(fullPath))
        {
            await fileStream.CopyToAsync(output, cancellationToken);
        }

        var relativePath = string.IsNullOrEmpty(normalizedFolder)
            ? safeFileName
            : $"{normalizedFolder}/{safeFileName}";

        logger.LogInformation(
            "File uploaded to local storage. BasePath: {BasePath}. RelativePath: {RelativePath}",
            basePath,
            relativePath);

        return BuildPublicUrl(relativePath);
    }

    public Task DeleteAsync(string fileUrlOrPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrlOrPath))
            return Task.CompletedTask;

        try
        {
            var basePath = ResolveBasePath();
            var relative = ExtractRelativePath(fileUrlOrPath);

            if (string.IsNullOrWhiteSpace(relative))
                return Task.CompletedTask;

            var fullPath = Path.Combine(basePath, relative.Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error deleting local file. File: {File}", fileUrlOrPath);
        }

        return Task.CompletedTask;
    }

    private string ResolveBasePath()
    {
        var configured = string.IsNullOrWhiteSpace(_options.LocalBasePath)
            ? "wwwroot/uploads"
            : _options.LocalBasePath;

        return Path.IsPathRooted(configured)
            ? configured
            : Path.Combine(Directory.GetCurrentDirectory(), configured);
    }

    private string BuildPublicUrl(string relativePath)
    {
        var normalizedRelative = relativePath.Replace('\\', '/').TrimStart('/');

        if (!string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
            return $"{_options.PublicBaseUrl.TrimEnd('/')}/{normalizedRelative}";

        return $"/uploads/{normalizedRelative}";
    }

    private string ExtractRelativePath(string fileUrlOrPath)
    {
        if (Uri.TryCreate(fileUrlOrPath, UriKind.Absolute, out var uri))
        {
            var path = uri.AbsolutePath.TrimStart('/');

            if (path.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
                return path["uploads/".Length..];

            return path;
        }

        var trimmed = fileUrlOrPath.Replace('\\', '/').TrimStart('/');

        if (trimmed.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
            return trimmed["uploads/".Length..];

        return trimmed;
    }

    private static string NormalizeFolder(string folder)
    {
        if (string.IsNullOrWhiteSpace(folder))
            return string.Empty;

        var segments = folder
            .Replace('\\', '/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Select(SanitizeSegment);

        return string.Join('/', segments);
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

    private static string SanitizeSegment(string segment)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sanitized = new string(segment.Where(c => !invalid.Contains(c)).ToArray());

        return string.IsNullOrWhiteSpace(sanitized) ? "_" : sanitized;
    }
}
