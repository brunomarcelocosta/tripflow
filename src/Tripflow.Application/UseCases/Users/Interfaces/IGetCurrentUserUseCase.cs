using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Users;

namespace Tripflow.Application.UseCases.Users.Interfaces;

public interface IGetCurrentUserUseCase
{
    Task<Result<GetCurrentUserResponse>> ExecuteAsync(CancellationToken cancellationToken = default);
}
