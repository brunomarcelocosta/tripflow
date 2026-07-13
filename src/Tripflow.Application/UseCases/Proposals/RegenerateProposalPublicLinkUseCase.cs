using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Proposals;
using Tripflow.Application.Helpers;
using Tripflow.Application.Options;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Proposals;

public class RegenerateProposalPublicLinkUseCase(
    IProposalRepository proposalRepository,
    IOptions<FrontendOptions> frontendOptions,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<RegenerateProposalPublicLinkUseCase> logger) : IRegenerateProposalPublicLinkUseCase
{
    public async Task<Result<ProposalResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<ProposalResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.ProposalsWrite, cancellationToken))
            return Result<ProposalResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var proposal = await proposalRepository.GetTrackedByIdAndTenantAsync(id, tenantId, cancellationToken);
        if (proposal is null)
            return Result<ProposalResponse?>.Failure("Proposta não encontrada.");

        try
        {
            var token = Guid.NewGuid().ToString("N");
            var url = ProposalPublicUrlHelper.BuildPublicUrl(token, frontendOptions.Value);
            proposal.SetPublicLink(token, url, updatedBy);
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
            logger.LogError("RegenerateProposalPublicLinkUseCase | Erro | {Message}", ex.Message);
            return Result<ProposalResponse?>.Failure("Erro inesperado ao regenerar link público.");
        }
    }
}
