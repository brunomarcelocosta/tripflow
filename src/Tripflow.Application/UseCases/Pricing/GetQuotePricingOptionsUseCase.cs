using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Pricing;
using Tripflow.Application.UseCases.Pricing.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Pricing;

public class GetQuotePricingOptionsUseCase(
    IQuoteRepository quoteRepository,
    IQuotePricingOptionRepository pricingRepository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetQuotePricingOptionsUseCase
{
    public async Task<Result<IEnumerable<QuotePricingOptionResponse>>> ExecuteAsync(Guid quoteId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<IEnumerable<QuotePricingOptionResponse>>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<IEnumerable<QuotePricingOptionResponse>>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesRead, cancellationToken))
            return Result<IEnumerable<QuotePricingOptionResponse>>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<IEnumerable<QuotePricingOptionResponse>>.Failure("Cotação não encontrada.");

        var options = await pricingRepository.GetByQuoteIdAsync(quoteId, tenantId, cancellationToken);
        var mapped = options.Select(mapper.Map<QuotePricingOptionResponse>).ToList();
        return Result<IEnumerable<QuotePricingOptionResponse>>.Ok(mapped);
    }
}
