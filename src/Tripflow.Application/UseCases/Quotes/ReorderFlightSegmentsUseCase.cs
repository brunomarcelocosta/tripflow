using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class ReorderFlightSegmentsUseCase(
    IQuoteRepository quoteRepository,
    IQuoteFlightItemRepository flightRepository,
    IFlightSegmentRepository segmentRepository,
    IValidator<ReorderFlightSegmentsRequest> validator,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<ReorderFlightSegmentsUseCase> logger) : IReorderFlightSegmentsUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid quoteId, Guid flightItemId, ReorderFlightSegmentsRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<bool>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<bool>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<bool>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<bool>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<bool>.Failure("Cotação não pode ser alterada no status atual.");

        var flightItem = await flightRepository.GetByIdAndQuoteAsync(flightItemId, quoteId, tenantId, cancellationToken);
        if (flightItem is null)
            return Result<bool>.Failure("Voo não encontrado.");

        var segments = await segmentRepository.GetTrackedByFlightItemIdAsync(flightItem.Id, tenantId, cancellationToken);
        var byId = segments.ToDictionary(s => s.Id);

        try
        {
            foreach (var item in request.Segments)
            {
                if (!byId.TryGetValue(item.SegmentId, out var segment))
                    return Result<bool>.Failure($"Segmento {item.SegmentId} não pertence ao voo.");
                segment.UpdateSequence(item.Sequence, updatedBy);
                await segmentRepository.UpdateAsync(segment, cancellationToken);
            }
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("ReorderFlightSegmentsUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao reordenar segmentos.");
        }
    }
}
