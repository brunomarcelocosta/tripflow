using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.UseCases.Payments;

internal static class PaymentAgencyCostHelper
{
    public static async Task<decimal?> ResolveAsync(
        Payment payment,
        IProposalRepository proposalRepository,
        IQuotePricingOptionRepository pricingRepository,
        CancellationToken cancellationToken)
    {
        if (!payment.ProposalId.HasValue)
            return null;

        var proposal = await proposalRepository.GetByIdAndTenantAsync(payment.ProposalId.Value, payment.TenantId, cancellationToken);
        if (proposal?.QuotePricingOptionId is not { } optionId)
            return null;

        var option = await pricingRepository.GetByIdAndQuoteAsync(optionId, proposal.QuoteId, payment.TenantId, cancellationToken);
        return option?.AgencyCost;
    }
}
