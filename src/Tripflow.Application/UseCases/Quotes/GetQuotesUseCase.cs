using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Quotes;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class GetQuotesUseCase(
    IQuoteRepository repository,
    ICustomerRepository customerRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetQuotesUseCase
{
    public async Task<Result<PagedResponse<QuoteResponse>>> ExecuteAsync(QuoteFilterRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<QuoteResponse>>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<PagedResponse<QuoteResponse>>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesRead, cancellationToken))
            return Result<PagedResponse<QuoteResponse>>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;

        var orderBy = QuoteOrderByHelper.Build(request.SortBy);
        var filter = request.ToExpression();

        var paged = await repository.GetPagedAsync(filter, request.Page, request.PageSize, orderBy, request.SortDesc);

        var quoteIds = paged.Items.Select(x => x.Id).ToList();
        var aggregates = await repository.GetAggregatesAsync(tenantId, quoteIds, cancellationToken);

        var customerIds = paged.Items
            .Where(x => x.CustomerId.HasValue)
            .Select(x => x.CustomerId!.Value)
            .Distinct()
            .ToList();

        var customers = new Dictionary<Guid, string>();
        foreach (var customerId in customerIds)
        {
            var customer = await customerRepository.GetByIdAndTenantAsync(customerId, tenantId, cancellationToken);
            if (customer != null)
                customers[customer.Id] = customer.FullName;
        }

        var mapped = paged.Items.Select(q =>
        {
            var agg = aggregates.TryGetValue(q.Id, out var a) ? a : (0, 0, false);
            return new QuoteResponse(
                q.Id,
                q.TenantId,
                q.CustomerId,
                q.CustomerId.HasValue && customers.TryGetValue(q.CustomerId.Value, out var name) ? name : null,
                q.QuoteNumber,
                q.Title,
                q.Type,
                q.Status,
                q.Origin,
                q.Destination,
                q.DepartureDate,
                q.ReturnDate,
                q.Adults,
                q.Children,
                q.Infants,
                q.Notes,
                q.ExpiresAtUtc,
                agg.Item1,
                agg.Item2,
                agg.Item3,
                q.CreatedAtUtc,
                q.UpdatedAtUtc);
        }).ToList();

        var response = new PagedResponse<QuoteResponse>(mapped, paged.Page, paged.PageSize, paged.TotalItems, paged.TotalPages);
        return Result<PagedResponse<QuoteResponse>>.Ok(response);
    }
}
