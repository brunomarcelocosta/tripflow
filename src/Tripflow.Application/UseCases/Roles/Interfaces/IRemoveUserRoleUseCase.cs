using Tripflow.Application.DTOs.Common;

namespace Tripflow.Application.UseCases.Roles.Interfaces;

public interface IRemoveUserRoleUseCase
{
    Task<Result<bool>> ExecuteAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default);
}
