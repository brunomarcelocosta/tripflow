using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Pricing;

namespace Tripflow.Application.UseCases.Pricing.Interfaces;

public interface IGetPaymentFeeRulesUseCase
{
    Task<Result<List<PaymentFeeRuleResponse>>> ExecuteAsync(CancellationToken cancellationToken = default);
}
