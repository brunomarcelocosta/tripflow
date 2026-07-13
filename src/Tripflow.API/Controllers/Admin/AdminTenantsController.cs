using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.UseCases.Admin.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Admin;

[ApiController]
[Route("api/admin/tenants")]
[Authorize]
public sealed class AdminTenantsController(
    IGetAdminTenantsUseCase getAdminTenantsUseCase,
    IGetAdminTenantByIdUseCase getAdminTenantByIdUseCase,
    IUpdateAdminTenantUseCase updateAdminTenantUseCase,
    IActivateAdminTenantUseCase activateAdminTenantUseCase,
    ISuspendAdminTenantUseCase suspendAdminTenantUseCase,
    ICancelAdminTenantUseCase cancelAdminTenantUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.TenantsRead)]
    public async Task<IActionResult> GetAll([FromQuery] AdminTenantFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getAdminTenantsUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{tenantId:guid}")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.TenantsRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid tenantId, CancellationToken cancellationToken)
    {
        var result = await getAdminTenantByIdUseCase.ExecuteAsync(tenantId, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        if (result.Data is null)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut("{tenantId:guid}")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.TenantsManage)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid tenantId,
        [FromBody] UpdateTenantRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateAdminTenantUseCase.ExecuteAsync(tenantId, request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        if (result.Data is null)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPatch("{tenantId:guid}/activate")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.TenantsManage)]
    public async Task<IActionResult> Activate([FromRoute] Guid tenantId, CancellationToken cancellationToken)
    {
        var result = await activateAdminTenantUseCase.ExecuteAsync(tenantId, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPatch("{tenantId:guid}/suspend")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.TenantsManage)]
    public async Task<IActionResult> Suspend([FromRoute] Guid tenantId, CancellationToken cancellationToken)
    {
        var result = await suspendAdminTenantUseCase.ExecuteAsync(tenantId, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPatch("{tenantId:guid}/cancel")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.TenantsManage)]
    public async Task<IActionResult> Cancel([FromRoute] Guid tenantId, CancellationToken cancellationToken)
    {
        var result = await cancelAdminTenantUseCase.ExecuteAsync(tenantId, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
