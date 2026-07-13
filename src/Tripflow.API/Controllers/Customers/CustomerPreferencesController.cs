using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Application.UseCases.Customers.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Customers;

[ApiController]
[Route("api/customers/{customerId:guid}/preferences")]
[Authorize]
public sealed class CustomerPreferencesController(
    IGetCustomerPreferenceUseCase getCustomerPreferenceUseCase,
    IUpdateCustomerPreferenceUseCase updateCustomerPreferenceUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.CustomersRead)]
    public async Task<IActionResult> Get([FromRoute] Guid customerId, CancellationToken cancellationToken)
    {
        var result = await getCustomerPreferenceUseCase.ExecuteAsync(customerId, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut]
    [RequirePermission(TripflowDbSeedData.Permissions.CustomersWrite)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid customerId,
        [FromBody] UpdateCustomerPreferenceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateCustomerPreferenceUseCase.ExecuteAsync(customerId, request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
