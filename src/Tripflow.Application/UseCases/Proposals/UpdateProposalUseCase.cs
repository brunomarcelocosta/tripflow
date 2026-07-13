using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Proposals;
using Tripflow.Application.DTOs.Responses.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Proposals;

public class UpdateProposalUseCase(
    IProposalRepository proposalRepository,
    IQuotePricingOptionRepository pricingRepository,
    IValidator<UpdateProposalRequest> validator,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<UpdateProposalUseCase> logger) : IUpdateProposalUseCase
{
    public async Task<Result<ProposalResponse?>> ExecuteAsync(Guid id, UpdateProposalRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<ProposalResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.ProposalsWrite, cancellationToken))
            return Result<ProposalResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<ProposalResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var proposal = await proposalRepository.GetTrackedByIdAndTenantAsync(id, tenantId, cancellationToken);
        if (proposal is null)
            return Result<ProposalResponse?>.Failure("Proposta não encontrada.");
        if (!proposal.CanBeUpdated())
            return Result<ProposalResponse?>.Failure("Proposta não pode ser alterada no status atual.");

        if (request.QuotePricingOptionId.HasValue)
        {
            var option = await pricingRepository.GetByIdAndQuoteAsync(request.QuotePricingOptionId.Value, proposal.QuoteId, tenantId, cancellationToken);
            if (option is null)
                return Result<ProposalResponse?>.Failure("Opção de preço não encontrada para a cotação.");
        }

        try
        {
            proposal.Update(request.QuotePricingOptionId, request.ExpiresAtUtc, updatedBy);
            await proposalRepository.UpdateAsync(proposal, cancellationToken);

            var aggregates = await proposalRepository.GetAggregatesAsync(tenantId, [id], cancellationToken);
            var agg = aggregates.TryGetValue(id, out var a) ? a : (0, 0);

            return Result<ProposalResponse?>.Ok(new ProposalResponse(
                proposal.Id, proposal.TenantId, proposal.QuoteId, proposal.QuotePricingOptionId, proposal.ProposalNumber, proposal.Status,
                proposal.PublicToken, proposal.PublicUrl, proposal.PdfUrl, proposal.ViewedAtUtc, proposal.ApprovedAtUtc, proposal.RejectedAtUtc, proposal.ExpiresAtUtc,
                agg.Item1, agg.Item2, proposal.CreatedAtUtc, proposal.UpdatedAtUtc));
        }
        catch (Exception ex)
        {
            logger.LogError("UpdateProposalUseCase | Erro | {Message}", ex.Message);
            return Result<ProposalResponse?>.Failure("Erro inesperado ao atualizar proposta.");
        }
    }
}
