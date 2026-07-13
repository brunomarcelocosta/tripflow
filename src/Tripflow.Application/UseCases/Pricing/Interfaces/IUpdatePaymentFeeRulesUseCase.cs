using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Pricing;
using Tripflow.Application.DTOs.Responses.Pricing;

namespace Tripflow.Application.UseCases.Pricing.Interfaces;

public interface IUpdatePaymentFeeRulesUseCase
{
    Task<Result<List<PaymentFeeRuleResponse>>> ExecuteAsync(
        UpdatePaymentFeeRulesRequest request,
        CancellationToken cancellationToken = default);
}
