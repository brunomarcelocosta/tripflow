using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.DTOs.Responses.Quotes;
using Tripflow.Application.Services.Subscriptions;
using Tripflow.Application.UseCases.Quotes.Interfaces;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Quotes;

public class CreateQuoteUseCase(
    IQuoteRepository repository,
    ICustomerRepository customerRepository,
    IValidator<CreateQuoteRequest> validator,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ITenantUsageService tenantUsageService,
    ILogger<CreateQuoteUseCase> logger) : ICreateQuoteUseCase
{
    public string ClassName = nameof(CreateQuoteUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<QuoteResponse?>> ExecuteAsync(CreateQuoteRequest request, CancellationToken cancellationToken = default)
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
        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var hasQuoteLimit = await tenantUsageService.HasAvailableLimitAsync(ITenantUsageService.Quotes, cancellationToken);
        if (!hasQuoteLimit)
            return Result<QuoteResponse?>.Failure("Limite de cotações do plano atingido.");

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
            var quoteNumber = await repository.GenerateNextQuoteNumberAsync(tenantId, cancellationToken);

            var quote = new Quote(
                tenantId,
                request.CustomerId,
                quoteNumber,
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
                createdByUserId: null,
                createdBy);

            await repository.AddAsync(quote, cancellationToken);

            var incremented = await tenantUsageService.IncrementAsync(ITenantUsageService.Quotes, cancellationToken: cancellationToken);
            if (!incremented)
                return Result<QuoteResponse?>.Failure("Não foi possível atualizar o consumo de cotações.");

            logger.LogInformation("{ClassName} | {Method} | Cotação criada {QuoteNumber}", ClassName, Method, quoteNumber);

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
                0,
                0,
                false,
                quote.CreatedAtUtc,
                quote.UpdatedAtUtc);

            return Result<QuoteResponse?>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError("{ClassName} | {Method} | Erro ao criar cotação | {Message}", ClassName, Method, ex.Message);
            return Result<QuoteResponse?>.Failure("Erro inesperado ao criar cotação.");
        }
    }
}
