using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Pricing;
using Tripflow.Application.DTOs.Responses.Pricing;
using Tripflow.Application.Helpers;
using Tripflow.Application.Services.Pricing;
using Tripflow.Application.UseCases.Pricing.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Pricing;

public class UpdateQuotePricingOptionUseCase(
    IQuoteRepository quoteRepository,
    IQuotePricingOptionRepository pricingRepository,
    IQuotePaymentConditionRepository paymentConditionRepository,
    ITenantCommercialSettingsRepository commercialSettingsRepository,
    IQuotePricingCalculatorService calculatorService,
    IValidator<UpdateQuotePricingOptionRequest> validator,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<UpdateQuotePricingOptionUseCase> logger) : IUpdateQuotePricingOptionUseCase
{
    public async Task<Result<QuotePricingOptionResponse?>> ExecuteAsync(Guid quoteId, Guid pricingOptionId, UpdateQuotePricingOptionRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<QuotePricingOptionResponse?>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<QuotePricingOptionResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<QuotePricingOptionResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<QuotePricingOptionResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<QuotePricingOptionResponse?>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<QuotePricingOptionResponse?>.Failure("Cotação não pode ser alterada no status atual.");

        var option = await pricingRepository.GetTrackedByIdAndQuoteAsync(pricingOptionId, quoteId, tenantId, cancellationToken);
        if (option is null)
            return Result<QuotePricingOptionResponse?>.Failure("Opção de precificação não encontrada.");

        try
        {
            var commercialSettings = await commercialSettingsRepository.GetByTenantIdAsync(tenantId, cancellationToken);
            var amounts = calculatorService.CalculateBaseAmounts(
                request.AgencyCost,
                request.DesiredProfitAmount,
                request.DesiredProfitPercentage,
                request.PixDiscountPercentage,
                commercialSettings);

            option.Rename(request.Name, updatedBy);
            option.UpdateAmounts(
                request.AgencyCost,
                request.DesiredProfitAmount,
                request.DesiredProfitPercentage,
                amounts.PixDiscountPercentage,
                amounts.PixAmount,
                amounts.CreditCashAmount,
                request.InternalNotes,
                updatedBy);

            await pricingRepository.UpdateAsync(option, cancellationToken);

            if (request.RegeneratePaymentConditions)
            {
                await paymentConditionRepository.DeleteByPricingOptionIdAsync(option.Id, tenantId, cancellationToken);
                var conditions = await calculatorService.GeneratePaymentConditionsAsync(option, updatedBy, cancellationToken);
                if (conditions.Count > 0)
                    await paymentConditionRepository.AddRangeAsync(conditions, cancellationToken);
            }

            var fresh = await pricingRepository.GetByIdAndQuoteAsync(option.Id, quoteId, tenantId, cancellationToken);
            return Result<QuotePricingOptionResponse?>.Ok(mapper.Map<QuotePricingOptionResponse>(fresh!));
        }
        catch (Exception ex)
        {
            logger.LogError("UpdateQuotePricingOptionUseCase | Erro | {Message}", ex.Message);
            return Result<QuotePricingOptionResponse?>.Failure("Erro inesperado ao atualizar opção de precificação.");
        }
    }
}
