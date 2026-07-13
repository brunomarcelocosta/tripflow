using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.DTOs.Responses.Miles;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Miles.Interfaces;
using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Miles;

public class CreateMilesQuoteOptionUseCase(
    IQuoteRepository quoteRepository,
    IMilesQuoteOptionRepository repository,
    ILoyaltyProgramRepository loyaltyProgramRepository,
    IValidator<CreateMilesQuoteOptionRequest> validator,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<CreateMilesQuoteOptionUseCase> logger) : ICreateMilesQuoteOptionUseCase
{
    public async Task<Result<MilesQuoteOptionResponse?>> ExecuteAsync(Guid quoteId, CreateMilesQuoteOptionRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<MilesQuoteOptionResponse?>.Forbidden();
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<MilesQuoteOptionResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<MilesQuoteOptionResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<MilesQuoteOptionResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<MilesQuoteOptionResponse?>.Failure("Cotação não encontrada.");
        if (QuoteStatusGuard.IsLocked(quote))
            return Result<MilesQuoteOptionResponse?>.Failure("Cotação não pode ser alterada no status atual.");

        if (request.LoyaltyProgramId.HasValue)
        {
            var program = await loyaltyProgramRepository.GetByIdAsync(request.LoyaltyProgramId.Value);
            if (program is null)
                return Result<MilesQuoteOptionResponse?>.Failure("Programa de fidelidade não encontrado.");
        }

        try
        {
            var option = new MilesQuoteOption(
                tenantId, quoteId, request.LoyaltyProgramId, request.Name,
                request.MilesAmount, request.BoardingFees, request.CostPerThousand,
                equivalentCost: null, request.CashPrice, estimatedSavings: null,
                request.ServiceFee, totalAmount: null, selected: false, request.Notes, createdBy);

            option.RecalculateAmounts(createdBy);

            await repository.AddAsync(option, cancellationToken);

            var fresh = await repository.GetByIdAndQuoteAsync(option.Id, quoteId, tenantId, cancellationToken);
            return Result<MilesQuoteOptionResponse?>.Ok(mapper.Map<MilesQuoteOptionResponse>(fresh!));
        }
        catch (Exception ex)
        {
            logger.LogError("CreateMilesQuoteOptionUseCase | Erro | {Message}", ex.Message);
            return Result<MilesQuoteOptionResponse?>.Failure("Erro inesperado ao criar opção de milhas.");
        }
    }
}
