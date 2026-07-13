using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.DTOs.Responses.Quotes;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class UpdateQuoteFlightItemUseCase(
    IQuoteRepository quoteRepository,
    IQuoteFlightItemRepository flightRepository,
    IValidator<UpdateQuoteFlightItemRequest> validator,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<UpdateQuoteFlightItemUseCase> logger) : IUpdateQuoteFlightItemUseCase
{
    public async Task<Result<QuoteFlightItemResponse?>> ExecuteAsync(Guid quoteId, Guid flightItemId, UpdateQuoteFlightItemRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<QuoteFlightItemResponse?>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<QuoteFlightItemResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<QuoteFlightItemResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<QuoteFlightItemResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<QuoteFlightItemResponse?>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<QuoteFlightItemResponse?>.Failure("Cotação não pode ser alterada no status atual.");

        var item = await flightRepository.GetTrackedByIdAndQuoteAsync(flightItemId, quoteId, tenantId, cancellationToken);
        if (item is null)
            return Result<QuoteFlightItemResponse?>.Failure("Voo não encontrado.");

        try
        {
            item.Update(
                request.AirlineName, request.FareFamily, request.BaggageDescription,
                request.IncludedPersonalItem, request.IncludedCarryOn, request.CarryOnWeightKg,
                request.IncludedCheckedBag, request.CheckedBagWeightKg, updatedBy);
            await flightRepository.UpdateAsync(item, cancellationToken);

            var fresh = await flightRepository.GetByIdAndQuoteAsync(item.Id, quoteId, tenantId, cancellationToken);
            return Result<QuoteFlightItemResponse?>.Ok(mapper.Map<QuoteFlightItemResponse>(fresh!));
        }
        catch (Exception ex)
        {
            logger.LogError("UpdateQuoteFlightItemUseCase | Erro | {Message}", ex.Message);
            return Result<QuoteFlightItemResponse?>.Failure("Erro inesperado ao atualizar voo.");
        }
    }
}
