using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.DTOs.Responses.Quotes;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class UpdateQuoteUseCase(
    IQuoteRepository repository,
    ICustomerRepository customerRepository,
    IValidator<UpdateQuoteRequest> validator,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<UpdateQuoteUseCase> logger) : IUpdateQuoteUseCase
{
    public async Task<Result<QuoteResponse?>> ExecuteAsync(Guid id, UpdateQuoteRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<QuoteResponse?>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<QuoteResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.QuotesWrite, cancellationToken))
            return Result<QuoteResponse?>.Forbidden();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault();
            return Result<QuoteResponse?>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var quote = await repository.GetTrackedByIdAndTenantAsync(id, tenantId, cancellationToken);
        if (quote is null)
            return Result<QuoteResponse?>.Failure("Cotação não encontrada.");

        if (!quote.CanBeUpdated())
            return Result<QuoteResponse?>.Failure("Cotação não pode ser alterada no status atual.");

        string? customerName = null;
        if (request.CustomerId.HasValue)
        {
            var customer = await customerRepository.GetByIdAndTenantAsync(request.CustomerId.Value, tenantId, cancellationToken);
            if (customer is null)
                return Result<QuoteResponse?>.Failure("Cliente informado não encontrado para o tenant atual.");
            customerName = customer.FullName;
        }

        try
        {
            quote.Update(
                request.CustomerId,
                request.Title,
                request.Type,
                request.Origin,
                request.Destination,
                request.DepartureDate,
                request.ReturnDate,
                request.Adults,
                request.Children,
                request.Infants,
                request.Notes,
                request.ExpiresAtUtc,
                updatedBy);

            await repository.UpdateAsync(quote, cancellationToken);

            var aggregates = await repository.GetAggregatesAsync(tenantId, new[] { quote.Id }, cancellationToken);
            var agg = aggregates.TryGetValue(quote.Id, out var a) ? a : (0, 0, false);

            var response = new QuoteResponse(
                quote.Id,
                quote.TenantId,
                quote.CustomerId,
                customerName,
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
        catch (Exception ex)
        {
            logger.LogError("UpdateQuoteUseCase | Erro | {Message}", ex.Message);
            return Result<QuoteResponse?>.Failure("Erro inesperado ao atualizar cotação.");
        }
    }
}
