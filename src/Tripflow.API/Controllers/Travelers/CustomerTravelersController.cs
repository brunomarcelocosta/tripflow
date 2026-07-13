using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Travelers;
using Tripflow.Application.UseCases.Travelers.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Travelers;

[ApiController]
[Route("api/customers/{customerId:guid}/travelers")]
[Authorize]
public sealed class CustomerTravelersController(
    IGetCustomerTravelersUseCase getCustomerTravelersUseCase,
    IGetTravelerByIdUseCase getTravelerByIdUseCase,
    ICreateTravelerUseCase createTravelerUseCase,
    IUpdateTravelerUseCase updateTravelerUseCase,
    IDeleteTravelerUseCase deleteTravelerUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.TravelersRead)]
    public async Task<IActionResult> GetAll(
        [FromRoute] Guid customerId,
        [FromQuery] TravelerFilterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await getCustomerTravelersUseCase.ExecuteAsync(customerId, request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{travelerId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.TravelersRead)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid customerId,
        [FromRoute] Guid travelerId,
        CancellationToken cancellationToken)
    {
        var result = await getTravelerByIdUseCase.ExecuteAsync(travelerId, customerId, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        if (result.Data is null)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    [RequirePermission(TripflowDbSeedData.Permissions.TravelersWrite)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid customerId,
        [FromBody] CreateTravelerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createTravelerUseCase.ExecuteAsync(customerId, request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("{travelerId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.TravelersWrite)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid customerId,
        [FromRoute] Guid travelerId,
        [FromBody] UpdateTravelerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateTravelerUseCase.ExecuteAsync(customerId, travelerId, request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{travelerId:guid}")]
    [RequirePermission(TripflowDbSeedData.Permissions.TravelersWrite)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid customerId,
        [FromRoute] Guid travelerId,
        CancellationToken cancellationToken)
    {
        var result = await deleteTravelerUseCase.ExecuteAsync(customerId, travelerId, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
