using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Pricing;
using Tripflow.Application.DTOs.Responses.Pricing;

namespace Tripflow.Application.UseCases.Pricing.Interfaces;

public interface IGetQuotePricingOptionsUseCase
{
    Task<Result<IEnumerable<QuotePricingOptionResponse>>> ExecuteAsync(Guid quoteId, CancellationToken cancellationToken = default);
}

public interface IGetQuotePricingOptionByIdUseCase
{
    Task<Result<QuotePricingOptionResponse?>> ExecuteAsync(Guid quoteId, Guid pricingOptionId, CancellationToken cancellationToken = default);
}

public interface ICreateQuotePricingOptionUseCase
{
    Task<Result<QuotePricingOptionResponse?>> ExecuteAsync(Guid quoteId, CreateQuotePricingOptionRequest request, CancellationToken cancellationToken = default);
}

public interface IUpdateQuotePricingOptionUseCase
{
    Task<Result<QuotePricingOptionResponse?>> ExecuteAsync(Guid quoteId, Guid pricingOptionId, UpdateQuotePricingOptionRequest request, CancellationToken cancellationToken = default);
}

public interface IDeleteQuotePricingOptionUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid quoteId, Guid pricingOptionId, CancellationToken cancellationToken = default);
}

public interface ISelectQuotePricingOptionUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid quoteId, Guid pricingOptionId, CancellationToken cancellationToken = default);
}

public interface IGetQuotePaymentConditionsUseCase
{
    Task<Result<IEnumerable<QuotePaymentConditionResponse>>> ExecuteAsync(Guid quoteId, Guid pricingOptionId, CancellationToken cancellationToken = default);
}

public interface IRegenerateQuotePaymentConditionsUseCase
{
    Task<Result<IEnumerable<QuotePaymentConditionResponse>>> ExecuteAsync(Guid quoteId, Guid pricingOptionId, CancellationToken cancellationToken = default);
}
