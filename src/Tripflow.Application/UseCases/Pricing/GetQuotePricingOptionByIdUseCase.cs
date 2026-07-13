using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Pricing;
using Tripflow.Application.UseCases.Pricing.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Pricing;

public class GetQuotePricingOptionByIdUseCase(
    IQuoteRepository quoteRepository,
    IQuotePricingOptionRepository pricingRepository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetQuotePricingOptionByIdUseCase
{
    public async Task<Result<QuotePricingOptionResponse?>> ExecuteAsync(Guid quoteId, Guid pricingOptionId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<QuotePricingOptionResponse?>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<QuotePricingOptionResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesRead, cancellationToken))
            return Result<QuotePricingOptionResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<QuotePricingOptionResponse?>.Failure("Cotação não encontrada.");

        var option = await pricingRepository.GetByIdAndQuoteAsync(pricingOptionId, quoteId, tenantId, cancellationToken);
        if (option is null)
            return Result<QuotePricingOptionResponse?>.Failure("Opção de precificação não encontrada.");

        return Result<QuotePricingOptionResponse?>.Ok(mapper.Map<QuotePricingOptionResponse>(option));
    }
}
