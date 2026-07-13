using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Payments;
using Tripflow.Application.UseCases.Payments.Interfaces;

namespace Tripflow.API.Controllers.Payments;

[ApiController]
[Route("api/financial-transactions")]
[Authorize]
public sealed class FinancialTransactionsController(
    IGetFinancialTransactionsUseCase getFinancialTransactionsUseCase,
    IGetFinancialTransactionByIdUseCase getFinancialTransactionByIdUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] FinancialTransactionFilterRequest request, CancellationToken cancellationToken)
    {
        var result = await getFinancialTransactionsUseCase.ExecuteAsync(request, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await getFinancialTransactionByIdUseCase.ExecuteAsync(id, cancellationToken);
        if (result.IsForbidden) return Forbid();
        if (!result.Success) return BadRequest(result);
        if (result.Data is null) return NotFound(result);
        return Ok(result);
    }
}
