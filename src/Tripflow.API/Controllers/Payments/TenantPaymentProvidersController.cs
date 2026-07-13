using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Payments;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Payments;

[ApiController]
[Route("api/tenant-payment-providers")]
[Authorize]
public sealed class TenantPaymentProvidersController(
    IGetTenantPaymentProvidersUseCase getTenantPaymentProvidersUseCase,
    ICreateTenantPaymentProviderUseCase createTenantPaymentProviderUseCase,
    IUpdateTenantPaymentProviderUseCase updateTenantPaymentProviderUseCase,
    IDeleteTenantPaymentProviderUseCase deleteTenantPaymentProviderUseCase,
    ISetDefaultTenantPaymentProviderUseCase setDefaultTenantPaymentProviderUseCase,
    IActivateTenantPaymentProviderUseCase activateTenantPaymentProviderUseCase,
    IInactivateTenantPaymentProviderUseCase inactivateTenantPaymentProviderUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await getTenantPaymentProvidersUseCase.ExecuteAsync(cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Create([FromBody] CreateTenantPaymentProviderRequest request, CancellationToken cancellationToken)
    {
        var result = await createTenantPaymentProviderUseCase.ExecuteAsync(request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTenantPaymentProviderRequest request, CancellationToken cancellationToken)
    {
        var result = await updateTenantPaymentProviderUseCase.ExecuteAsync(id, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await deleteTenantPaymentProviderUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/set-default")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> SetDefault([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await setDefaultTenantPaymentProviderUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/activate")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Activate([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await activateTenantPaymentProviderUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/inactivate")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Inactivate([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await inactivateTenantPaymentProviderUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
