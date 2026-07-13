using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Proposals;
using Tripflow.Application.DTOs.Responses.Proposals;
using Tripflow.Application.Helpers;
using Tripflow.Application.Options;
using Tripflow.Application.Services.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Domain.Entities.Proposals;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Proposals;

public class CreateProposalFromQuoteUseCase(
    IQuoteRepository quoteRepository,
    IQuotePricingOptionRepository pricingRepository,
    ITenantCommercialSettingsRepository commercialSettingsRepository,
    IProposalRepository proposalRepository,
    IProposalDocumentService documentService,
    IProposalEventService eventService,
    IValidator<CreateProposalFromQuoteRequest> validator,
    IOptions<FrontendOptions> frontendOptions,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<CreateProposalFromQuoteUseCase> logger) : ICreateProposalFromQuoteUseCase
{
    public async Task<Result<ProposalResponse?>> ExecuteAsync(Guid quoteId, CreateProposalFromQuoteRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<ProposalResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.ProposalsWrite, cancellationToken))
            return Result<ProposalResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<ProposalResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await quoteRepository.GetByIdAndTenantAsync(quoteId, tenantId, cancellationToken);
        if (quote is null)
            return Result<ProposalResponse?>.Failure("Cotação não encontrada.");
        if (quote.Status == QuoteStatus.Cancelled)
            return Result<ProposalResponse?>.Failure("Cotação cancelada não pode gerar proposta.");

        var pricingOption = await ResolvePricingOptionAsync(quoteId, tenantId, request.QuotePricingOptionId, cancellationToken);
        if (pricingOption is null)
            return Result<ProposalResponse?>.Failure("Informe uma opção de preço ou selecione uma opção na cotação.");

        var expiresAt = request.ExpiresAtUtc;
        if (!expiresAt.HasValue)
        {
            var commercial = await commercialSettingsRepository.GetByTenantIdAsync(tenantId, cancellationToken);
            if (commercial?.DefaultProposalExpirationHours is > 0)
                expiresAt = DateTime.UtcNow.AddHours(commercial.DefaultProposalExpirationHours);
        }

        try
        {
            var proposalNumber = await proposalRepository.GenerateNextProposalNumberAsync(tenantId, cancellationToken);
            string? publicToken = null;
            string? publicUrl = null;
            if (request.GeneratePublicLink)
            {
                publicToken = Guid.NewGuid().ToString("N");
                publicUrl = ProposalPublicUrlHelper.BuildPublicUrl(publicToken, frontendOptions.Value);
            }

            var proposal = new Proposal(
                tenantId, quoteId, pricingOption.Id, proposalNumber,
                ProposalStatus.Draft, publicToken, publicUrl, null, expiresAt, createdBy);

            await proposalRepository.AddAsync(proposal, cancellationToken);

            if (request.GeneratePdf)
            {
                var tracked = await proposalRepository.GetTrackedByIdAndTenantAsync(proposal.Id, tenantId, cancellationToken);
                if (tracked is not null)
                    await documentService.GenerateVersionAsync(tracked, true, null, createdBy, cancellationToken);
            }

            var fresh = await proposalRepository.GetByIdAndTenantAsync(proposal.Id, tenantId, cancellationToken);
            var aggregates = await proposalRepository.GetAggregatesAsync(tenantId, [proposal.Id], cancellationToken);
            var agg = aggregates.TryGetValue(proposal.Id, out var a) ? a : (0, 0);

            return Result<ProposalResponse?>.Ok(new ProposalResponse(
                fresh!.Id, fresh.TenantId, fresh.QuoteId, fresh.QuotePricingOptionId, fresh.ProposalNumber, fresh.Status,
                fresh.PublicToken, fresh.PublicUrl, fresh.PdfUrl, fresh.ViewedAtUtc, fresh.ApprovedAtUtc, fresh.RejectedAtUtc, fresh.ExpiresAtUtc,
                agg.Item1, agg.Item2, fresh.CreatedAtUtc, fresh.UpdatedAtUtc));
        }
        catch (Exception ex)
        {
            logger.LogError("CreateProposalFromQuoteUseCase | Erro | {Message}", ex.Message);
            return Result<ProposalResponse?>.Failure("Erro inesperado ao criar proposta.");
        }
    }

    private async Task<Domain.Entities.Pricing.QuotePricingOption?> ResolvePricingOptionAsync(
        Guid quoteId, Guid tenantId, Guid? pricingOptionId, CancellationToken cancellationToken)
    {
        if (pricingOptionId.HasValue)
            return await pricingRepository.GetByIdAndQuoteAsync(pricingOptionId.Value, quoteId, tenantId, cancellationToken);

        var options = await pricingRepository.GetByQuoteIdAsync(quoteId, tenantId, cancellationToken);
        return options.FirstOrDefault(o => o.Selected);
    }
}
