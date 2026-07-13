using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Roles;
using Tripflow.Application.DTOs.Requests.Users;
using Tripflow.Application.UseCases.Roles.Interfaces;
using Tripflow.Application.UseCases.Users.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Users;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController(
    IGetCurrentUserUseCase getCurrentUserUseCase,
    IInviteUserUseCase inviteUserUseCase,
    IAssignUserRolesUseCase assignUserRolesUseCase,
    IRemoveUserRoleUseCase removeUserRoleUseCase) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var result = await getCurrentUserUseCase.ExecuteAsync(cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("invite")]
    [RequirePermission(TripflowDbSeedData.Permissions.UsersManage)]
    public async Task<IActionResult> Invite(
        [FromBody] InviteUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await inviteUserUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{userId:guid}/roles")]
    [RequirePermission(TripflowDbSeedData.Permissions.UsersManage)]
    public async Task<IActionResult> AssignRoles(
        [FromRoute] Guid userId,
        [FromBody] AssignUserRolesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await assignUserRolesUseCase.ExecuteAsync(userId, request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{userId:guid}/roles/{roleId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.UsersManage)]
    public async Task<IActionResult> RemoveRole(
        [FromRoute] Guid userId,
        [FromRoute] Guid roleId,
        CancellationToken cancellationToken)
    {
        var result = await removeUserRoleUseCase.ExecuteAsync(userId, roleId, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
