using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Miles.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Miles;

public class SelectMilesQuoteOptionUseCase(
    IQuoteRepository quoteRepository,
    IMilesQuoteOptionRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<SelectMilesQuoteOptionUseCase> logger) : ISelectMilesQuoteOptionUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid quoteId, Guid milesOptionId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<bool>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<bool>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<bool>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<bool>.Failure("Cotação não pode ser alterada no status atual.");

        var options = await repository.GetTrackedByQuoteIdAsync(quoteId, tenantId, cancellationToken);
        var target = options.FirstOrDefault(o => o.Id == milesOptionId);
        if (target is null)
            return Result<bool>.Failure("Opção de milhas não encontrada.");

        try
        {
            foreach (var other in options.Where(o => o.Id != milesOptionId && o.Selected))
            {
                other.Unselect(updatedBy);
                await repository.UpdateAsync(other, cancellationToken);
            }

            target.MarkAsSelected(updatedBy);
            await repository.UpdateAsync(target, cancellationToken);

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("SelectMilesQuoteOptionUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao selecionar opção de milhas.");
        }
    }
}
