using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.UseCases.Miles.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Miles;

[ApiController]
[Route("api/customers/{customerId:guid}/loyalty-accounts")]
[Authorize]
public sealed class CustomerLoyaltyAccountsController(
    IGetCustomerLoyaltyAccountsUseCase getCustomerLoyaltyAccountsUseCase,
    IGetCustomerLoyaltyAccountByIdUseCase getCustomerLoyaltyAccountByIdUseCase,
    ICreateCustomerLoyaltyAccountUseCase createCustomerLoyaltyAccountUseCase,
    IUpdateCustomerLoyaltyAccountUseCase updateCustomerLoyaltyAccountUseCase,
    IDeleteCustomerLoyaltyAccountUseCase deleteCustomerLoyaltyAccountUseCase,
    IActivateCustomerLoyaltyAccountUseCase activateCustomerLoyaltyAccountUseCase,
    IInactivateCustomerLoyaltyAccountUseCase inactivateCustomerLoyaltyAccountUseCase,
    ISuspendCustomerLoyaltyAccountUseCase suspendCustomerLoyaltyAccountUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesRead)]
    public async Task<IActionResult> GetAll([FromRoute] Guid customerId, [FromQuery] CustomerLoyaltyAccountFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getCustomerLoyaltyAccountsUseCase.ExecuteAsync(customerId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid customerId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await getCustomerLoyaltyAccountByIdUseCase.ExecuteAsync(customerId, id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesWrite)]
    public async Task<IActionResult> Create([FromRoute] Guid customerId, [FromBody] CreateCustomerLoyaltyAccountRequest request, CancellationToken cancellationToken)
    {
        var result = await createCustomerLoyaltyAccountUseCase.ExecuteAsync(customerId, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesWrite)]
    public async Task<IActionResult> Update([FromRoute] Guid customerId, [FromRoute] Guid id, [FromBody] UpdateCustomerLoyaltyAccountRequest request, CancellationToken cancellationToken)
    {
        var result = await updateCustomerLoyaltyAccountUseCase.ExecuteAsync(customerId, id, request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesWrite)]
    public async Task<IActionResult> Delete([FromRoute] Guid customerId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await deleteCustomerLoyaltyAccountUseCase.ExecuteAsync(customerId, id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/activate")]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesWrite)]
    public async Task<IActionResult> Activate([FromRoute] Guid customerId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await activateCustomerLoyaltyAccountUseCase.ExecuteAsync(customerId, id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/inactivate")]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesWrite)]
    public async Task<IActionResult> Inactivate([FromRoute] Guid customerId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await inactivateCustomerLoyaltyAccountUseCase.ExecuteAsync(customerId, id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/suspend")]
    [RequirePermission(TripflowDbSeedData.Permissions.MilesWrite)]
    public async Task<IActionResult> Suspend([FromRoute] Guid customerId, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await suspendCustomerLoyaltyAccountUseCase.ExecuteAsync(customerId, id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
