using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.UseCases.Admin.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Admin;

[ApiController]
[Route("api/admin")]
[Authorize]
public sealed class AdminUsersController(
    IGetAdminUsersUseCase getAdminUsersUseCase,
    IGetAdminTenantUsersUseCase getAdminTenantUsersUseCase,
    IGetAdminUserByIdUseCase getAdminUserByIdUseCase,
    IUpdateAdminUserUseCase updateAdminUserUseCase,
    ISetAdminUserPasswordUseCase setAdminUserPasswordUseCase,
    IActivateAdminUserUseCase activateAdminUserUseCase,
    IBlockAdminUserUseCase blockAdminUserUseCase,
    IRemoveAdminUserUseCase removeAdminUserUseCase) : ControllerBase
{
    [HttpGet("users")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.UsersRead)]
    public async Task<IActionResult> GetAll(
        [FromQuery] AdminUserFilterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await getAdminUsersUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("tenants/{tenantId:guid}/users")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.UsersRead)]
    public async Task<IActionResult> GetByTenant(
        [FromRoute] Guid tenantId,
        [FromQuery] AdminUserFilterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await getAdminTenantUsersUseCase.ExecuteAsync(tenantId, request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("users/{userId:guid}")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.UsersRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await getAdminUserByIdUseCase.ExecuteAsync(userId, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        if (result.Data is null)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut("users/{userId:guid}")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.UsersManage)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid userId,
        [FromBody] UpdateAdminUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateAdminUserUseCase.ExecuteAsync(userId, request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        if (result.Data is null)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut("users/{userId:guid}/password")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.UsersManage)]
    public async Task<IActionResult> SetPassword(
        [FromRoute] Guid userId,
        [FromBody] SetAdminUserPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var result = await setAdminUserPasswordUseCase.ExecuteAsync(userId, request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        if (!result.Data)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPatch("users/{userId:guid}/activate")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.UsersManage)]
    public async Task<IActionResult> Activate([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await activateAdminUserUseCase.ExecuteAsync(userId, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPatch("users/{userId:guid}/block")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.UsersManage)]
    public async Task<IActionResult> Block([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await blockAdminUserUseCase.ExecuteAsync(userId, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPatch("users/{userId:guid}/remove")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.UsersManage)]
    public async Task<IActionResult> Remove([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await removeAdminUserUseCase.ExecuteAsync(userId, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
