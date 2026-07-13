namespace Tripflow.Domain.Interfaces;

public interface IUserPermissionService
{
    Task<bool> HasPermissionAsync(string permissionCode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetCurrentUserPermissionsAsync(CancellationToken cancellationToken = default);
}
