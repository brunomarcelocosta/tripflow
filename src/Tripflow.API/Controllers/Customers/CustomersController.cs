using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Application.UseCases.Customers.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Customers;

[ApiController]
[Route("api/customers")]
[Authorize]
public sealed class CustomersController(
    IGetCustomersUseCase getCustomersUseCase,
    IGetCustomerByIdUseCase getCustomerByIdUseCase,
    ICreateCustomerUseCase createCustomerUseCase,
    IUpdateCustomerUseCase updateCustomerUseCase,
    IDeleteCustomerUseCase deleteCustomerUseCase,
    IActivateCustomerUseCase activateCustomerUseCase,
    IInactivateCustomerUseCase inactivateCustomerUseCase,
    IBlockCustomerUseCase blockCustomerUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.CustomersRead)]
    public async Task<IActionResult> GetAll([FromQuery] CustomerFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getCustomersUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.CustomersRead)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await getCustomerByIdUseCase.ExecuteAsync(id, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        if (result.Data is null)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(TripflowDbSeedData.Permissions.CustomersWrite)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await createCustomerUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.CustomersWrite)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await updateCustomerUseCase.ExecuteAsync(id, request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.CustomersWrite)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await deleteCustomerUseCase.ExecuteAsync(id, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPatch("{id:guid}/activate")]
    [RequirePermission(TripflowDbSeedData.Permissions.CustomersWrite)]
    public async Task<IActionResult> Activate([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await activateCustomerUseCase.ExecuteAsync(id, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPatch("{id:guid}/inactivate")]
    [RequirePermission(TripflowDbSeedData.Permissions.CustomersWrite)]
    public async Task<IActionResult> Inactivate([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await inactivateCustomerUseCase.ExecuteAsync(id, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPatch("{id:guid}/block")]
    [RequirePermission(TripflowDbSeedData.Permissions.CustomersWrite)]
    public async Task<IActionResult> Block([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await blockCustomerUseCase.ExecuteAsync(id, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
