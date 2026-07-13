using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Roles;

namespace Tripflow.Application.UseCases.Roles.Interfaces;

public interface IGetRolesUseCase
{
    Task<Result<List<RoleResponse>>> ExecuteAsync(CancellationToken cancellationToken = default);
}
