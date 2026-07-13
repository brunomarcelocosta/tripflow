namespace Tripflow.Application.DTOs.Requests.Pricing;

public sealed class UpdatePaymentFeeRulesRequest
{
    public IEnumerable<UpdatePaymentFeeRuleItem> Rules { get; init; } = [];
}
