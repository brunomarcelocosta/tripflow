using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Pricing;
using Tripflow.Application.UseCases.Pricing.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Pricing;

public class GetQuotePaymentConditionsUseCase(
    IQuoteRepository quoteRepository,
    IQuotePricingOptionRepository pricingRepository,
    IQuotePaymentConditionRepository paymentConditionRepository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetQuotePaymentConditionsUseCase
{
    public async Task<Result<IEnumerable<QuotePaymentConditionResponse>>> ExecuteAsync(Guid quoteId, Guid pricingOptionId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<IEnumerable<QuotePaymentConditionResponse>>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<IEnumerable<QuotePaymentConditionResponse>>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesRead, cancellationToken))
            return Result<IEnumerable<QuotePaymentConditionResponse>>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<IEnumerable<QuotePaymentConditionResponse>>.Failure("Cotação não encontrada.");

        var option = await pricingRepository.GetByIdAndQuoteAsync(pricingOptionId, quoteId, tenantId, cancellationToken);
        if (option is null)
            return Result<IEnumerable<QuotePaymentConditionResponse>>.Failure("Opção de precificação não encontrada.");

        var conditions = await paymentConditionRepository.GetByPricingOptionIdAsync(option.Id, tenantId, cancellationToken);
        var mapped = conditions.Select(mapper.Map<QuotePaymentConditionResponse>).ToList();
        return Result<IEnumerable<QuotePaymentConditionResponse>>.Ok(mapped);
    }
}
