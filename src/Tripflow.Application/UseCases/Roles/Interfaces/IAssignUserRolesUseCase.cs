using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Roles;

namespace Tripflow.Application.UseCases.Roles.Interfaces;

public interface IAssignUserRolesUseCase
{
    Task<Result<IReadOnlyList<string>>> ExecuteAsync(
        Guid userId,
        AssignUserRolesRequest request,
        CancellationToken cancellationToken = default);
}
