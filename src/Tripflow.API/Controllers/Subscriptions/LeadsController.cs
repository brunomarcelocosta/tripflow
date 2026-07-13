using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.UseCases.Leads.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Subscriptions;

[ApiController]
[Route("api/leads")]
[Authorize]
public sealed class LeadsController(
    ICreateLeadUseCase createLeadUseCase,
    IGetLeadsUseCase getLeadsUseCase,
    IGetLeadByIdUseCase getLeadByIdUseCase,
    IConvertLeadToTenantUseCase convertLeadToTenantUseCase,
    IDeleteLeadUseCase deleteLeadUseCase) : ControllerBase
{
    [HttpPost("~/api/public/leads")]
    [AllowAnonymous]
    public async Task<IActionResult> CreatePublic([FromBody] CreateLeadRequest request, CancellationToken cancellationToken)
    {
        var result = await createLeadUseCase.ExecuteAsync(request, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Get([FromQuery] LeadFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getLeadsUseCase.ExecuteAsync(request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await getLeadByIdUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/convert-to-tenant")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> ConvertToTenant([FromRoute] Guid id, [FromBody] ConvertLeadToTenantRequest request, CancellationToken cancellationToken)
    {
        var result = await convertLeadToTenantUseCase.ExecuteAsync(id, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await deleteLeadUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
