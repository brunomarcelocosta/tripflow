using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class DeleteFlightSegmentUseCase(
    IQuoteRepository quoteRepository,
    IQuoteFlightItemRepository flightRepository,
    IFlightSegmentRepository segmentRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<DeleteFlightSegmentUseCase> logger) : IDeleteFlightSegmentUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid quoteId, Guid flightItemId, Guid segmentId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<bool>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<bool>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var deletedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<bool>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<bool>.Failure("Cotação não pode ser alterada no status atual.");

        var flightItem = await flightRepository.GetByIdAndQuoteAsync(flightItemId, quoteId, tenantId, cancellationToken);
        if (flightItem is null)
            return Result<bool>.Failure("Voo não encontrado.");

        var segment = await segmentRepository.GetTrackedByIdAndFlightItemAsync(segmentId, flightItem.Id, tenantId, cancellationToken);
        if (segment is null)
            return Result<bool>.Failure("Segmento não encontrado.");

        try
        {
            segment.SetDelete(deletedBy);
            await segmentRepository.UpdateAsync(segment, cancellationToken);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("DeleteFlightSegmentUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao remover segmento.");
        }
    }
}
