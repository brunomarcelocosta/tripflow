using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.UseCases.Tenants.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Tenants;

[ApiController]
[Route("api/tenants")]
[Authorize]
public sealed class TenantsController(ICreateTenantUseCase createTenantUseCase,
                                      IGetTenantByIdUseCase getTenantByIdUseCase,
                                      IGetTenantsUseCase getTenantsUseCase,
                                      IUpdateTenantUseCase updateTenantUseCase) : ControllerBase
{

    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.TenantsRead)]
    public async Task<IActionResult> GetAll([FromQuery] TenantFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getTenantsUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.TenantsRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await getTenantByIdUseCase.ExecuteAsync(new(id), cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        if (result.Data is null)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(TripflowDbSeedData.Permissions.TenantsWrite)]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequest request, CancellationToken cancellationToken)
    {
        var result = await createTenantUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.TenantsWrite)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTenantRequest request, CancellationToken cancellationToken)
    {
        var result = await updateTenantUseCase.ExecuteAsync(id, request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
