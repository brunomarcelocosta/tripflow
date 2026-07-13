using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Quotes;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class GetQuoteByIdUseCase(
    IQuoteRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetQuoteByIdUseCase
{
    public async Task<Result<QuoteResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<QuoteResponse?>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<QuoteResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesRead, cancellationToken))
            return Result<QuoteResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;

        var quote = await repository.GetByIdAndTenantAsync(id, tenantId, cancellationToken);

        if (quote is null)
            return Result<QuoteResponse?>.Ok(null);

        var aggregates = await repository.GetAggregatesAsync(tenantId, new[] { quote.Id }, cancellationToken);
        var agg = aggregates.TryGetValue(quote.Id, out var a) ? a : (0, 0, false);

        var response = new QuoteResponse(
            quote.Id,
            quote.TenantId,
            quote.CustomerId,
            quote.Customer?.FullName,
            quote.QuoteNumber,
            quote.Title,
            quote.Type,
            quote.Status,
            quote.Origin,
            quote.Destination,
            quote.DepartureDate,
            quote.ReturnDate,
            quote.Adults,
            quote.Children,
            quote.Infants,
            quote.Notes,
            quote.ExpiresAtUtc,
            agg.Item1,
            agg.Item2,
            agg.Item3,
            quote.CreatedAtUtc,
            quote.UpdatedAtUtc);

        return Result<QuoteResponse?>.Ok(response);
    }
}
