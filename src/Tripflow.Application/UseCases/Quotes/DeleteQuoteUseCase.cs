using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class DeleteQuoteUseCase(
    IQuoteRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<DeleteQuoteUseCase> logger) : IDeleteQuoteUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<bool>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<bool>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var deletedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await repository.GetTrackedByIdAndTenantAsync(id, tenantId, cancellationToken);
        if (quote is null)
            return Result<bool>.Failure("Cotação não encontrada.");

        try
        {
            quote.SetDelete(deletedBy);
            await repository.UpdateAsync(quote, cancellationToken);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("DeleteQuoteUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao remover cotação.");
        }
    }
}
