namespace Tripflow.Domain.Interfaces.Storage;

public interface IFileStorageService
{
    Task<string> UploadAsync(
        Stream fileStream,
        string folder,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string fileUrlOrPath,
        CancellationToken cancellationToken = default);
}
