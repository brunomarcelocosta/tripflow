using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Proposals;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Proposals;
using Tripflow.Application.Builders;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Proposals;

public class GetProposalsUseCase(
    IProposalRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetProposalsUseCase
{
    public async Task<Result<PagedResponse<ProposalResponse>>> ExecuteAsync(ProposalFilterRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<PagedResponse<ProposalResponse>>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.ProposalsRead, cancellationToken))
            return Result<PagedResponse<ProposalResponse>>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var filter = request.ToExpression();
        var orderBy = ProposalOrderByHelper.Build(request.SortBy);
        var paged = await repository.GetPagedAsync(filter, request.Page, request.PageSize, orderBy, request.SortDesc);

        var ids = paged.Items.Select(x => x.Id).ToList();
        var aggregates = await repository.GetAggregatesAsync(tenantId, ids, cancellationToken);

        var mapped = paged.Items.Select(p =>
        {
            var agg = aggregates.TryGetValue(p.Id, out var a) ? a : (0, 0);
            return new ProposalResponse(
                p.Id, p.TenantId, p.QuoteId, p.QuotePricingOptionId, p.ProposalNumber, p.Status,
                p.PublicToken, p.PublicUrl, p.PdfUrl, p.ViewedAtUtc, p.ApprovedAtUtc, p.RejectedAtUtc, p.ExpiresAtUtc,
                agg.Item1, agg.Item2, p.CreatedAtUtc, p.UpdatedAtUtc);
        }).ToList();

        return Result<PagedResponse<ProposalResponse>>.Ok(new PagedResponse<ProposalResponse>(mapped, paged.Page, paged.PageSize, paged.TotalItems, paged.TotalPages));
    }
}
