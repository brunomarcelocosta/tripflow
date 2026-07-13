using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Miles;
using Tripflow.Application.UseCases.Miles.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Miles;

public class GetMilesQuoteOptionsUseCase(
    IQuoteRepository quoteRepository,
    IMilesQuoteOptionRepository repository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetMilesQuoteOptionsUseCase
{
    public async Task<Result<IEnumerable<MilesQuoteOptionResponse>>> ExecuteAsync(Guid quoteId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<IEnumerable<MilesQuoteOptionResponse>>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<IEnumerable<MilesQuoteOptionResponse>>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesRead, cancellationToken))
            return Result<IEnumerable<MilesQuoteOptionResponse>>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<IEnumerable<MilesQuoteOptionResponse>>.Failure("Cotação não encontrada.");

        var options = await repository.GetByQuoteIdAsync(quoteId, tenantId, cancellationToken);
        var mapped = options.Select(mapper.Map<MilesQuoteOptionResponse>).ToList();
        return Result<IEnumerable<MilesQuoteOptionResponse>>.Ok(mapped);
    }
}
