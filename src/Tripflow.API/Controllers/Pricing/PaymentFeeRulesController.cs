using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Pricing;
using Tripflow.Application.UseCases.Pricing.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Pricing;

[ApiController]
[Route("api/payment-fee-rules")]
[Authorize]
public sealed class PaymentFeeRulesController(
    IGetPaymentFeeRulesUseCase getRulesUseCase,
    IUpdatePaymentFeeRulesUseCase updateRulesUseCase) : ControllerBase
{
    [HttpGet]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsRead)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await getRulesUseCase.ExecuteAsync(cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> Update(
        [FromBody] UpdatePaymentFeeRulesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateRulesUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
