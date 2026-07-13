using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Users;
using Tripflow.Application.DTOs.Responses.Users;

namespace Tripflow.Application.UseCases.Users.Interfaces;

public interface IInviteUserUseCase
{
    Task<Result<InviteUserResponse>> ExecuteAsync(
        InviteUserRequest request,
        CancellationToken cancellationToken = default);
}
