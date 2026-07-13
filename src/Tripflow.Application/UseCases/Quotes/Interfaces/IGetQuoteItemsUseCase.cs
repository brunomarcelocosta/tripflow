using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Quotes;

namespace Tripflow.Application.UseCases.Quotes.Interfaces;

public interface IGetQuoteItemsUseCase
{
    Task<Result<IEnumerable<QuoteItemResponse>>> ExecuteAsync(Guid quoteId, CancellationToken cancellationToken = default);
}

public interface IGetQuoteItemByIdUseCase
{
    Task<Result<QuoteItemResponse?>> ExecuteAsync(Guid quoteId, Guid itemId, CancellationToken cancellationToken = default);
}

public interface ICreateQuoteItemUseCase
{
    Task<Result<QuoteItemResponse?>> ExecuteAsync(Guid quoteId, DTOs.Requests.Quotes.CreateQuoteItemRequest request, CancellationToken cancellationToken = default);
}

public interface IUpdateQuoteItemUseCase
{
    Task<Result<QuoteItemResponse?>> ExecuteAsync(Guid quoteId, Guid itemId, DTOs.Requests.Quotes.UpdateQuoteItemRequest request, CancellationToken cancellationToken = default);
}

public interface IDeleteQuoteItemUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid quoteId, Guid itemId, CancellationToken cancellationToken = default);
}
