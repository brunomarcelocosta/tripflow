using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Proposals;

public class GetProposalByIdUseCase(
    IProposalRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetProposalByIdUseCase
{
    public async Task<Result<ProposalResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<ProposalResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.ProposalsRead, cancellationToken))
            return Result<ProposalResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var proposal = await repository.GetByIdAndTenantAsync(id, tenantId, cancellationToken);
        if (proposal is null)
            return Result<ProposalResponse?>.Ok(null);

        var aggregates = await repository.GetAggregatesAsync(tenantId, [id], cancellationToken);
        var agg = aggregates.TryGetValue(id, out var a) ? a : (0, 0);

        return Result<ProposalResponse?>.Ok(new ProposalResponse(
            proposal.Id, proposal.TenantId, proposal.QuoteId, proposal.QuotePricingOptionId, proposal.ProposalNumber, proposal.Status,
            proposal.PublicToken, proposal.PublicUrl, proposal.PdfUrl, proposal.ViewedAtUtc, proposal.ApprovedAtUtc, proposal.RejectedAtUtc, proposal.ExpiresAtUtc,
            agg.Item1, agg.Item2, proposal.CreatedAtUtc, proposal.UpdatedAtUtc));
    }
}
