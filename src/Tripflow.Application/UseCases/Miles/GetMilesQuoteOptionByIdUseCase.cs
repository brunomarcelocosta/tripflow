using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Miles;
using Tripflow.Application.UseCases.Miles.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Miles;

public class GetMilesQuoteOptionByIdUseCase(
    IQuoteRepository quoteRepository,
    IMilesQuoteOptionRepository repository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetMilesQuoteOptionByIdUseCase
{
    public async Task<Result<MilesQuoteOptionResponse?>> ExecuteAsync(Guid quoteId, Guid milesOptionId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<MilesQuoteOptionResponse?>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<MilesQuoteOptionResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesRead, cancellationToken))
            return Result<MilesQuoteOptionResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<MilesQuoteOptionResponse?>.Failure("Cotação não encontrada.");

        var option = await repository.GetByIdAndQuoteAsync(milesOptionId, quoteId, tenantId, cancellationToken);
        if (option is null)
            return Result<MilesQuoteOptionResponse?>.Failure("Opção de milhas não encontrada.");

        return Result<MilesQuoteOptionResponse?>.Ok(mapper.Map<MilesQuoteOptionResponse>(option));
    }
}
