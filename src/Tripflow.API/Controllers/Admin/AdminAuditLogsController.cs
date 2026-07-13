using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.UseCases.Admin.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Admin;

[ApiController]
[Route("api/admin/audit-logs")]
[Authorize]
public sealed class AdminAuditLogsController(
    IGetAdminAuditLogsUseCase getAdminAuditLogsUseCase,
    IGetAdminAuditLogByIdUseCase getAdminAuditLogByIdUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.AuditRead)]
    public async Task<IActionResult> GetAll([FromQuery] AdminAuditLogFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getAdminAuditLogsUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.PlatformPermissions.AuditRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await getAdminAuditLogByIdUseCase.ExecuteAsync(id, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        if (result.Data is null)
            return NotFound(result);

        return Ok(result);
    }
}
