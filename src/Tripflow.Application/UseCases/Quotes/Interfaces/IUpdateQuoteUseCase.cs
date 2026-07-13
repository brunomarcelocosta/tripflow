using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.DTOs.Responses.Quotes;

namespace Tripflow.Application.UseCases.Quotes.Interfaces;

public interface IUpdateQuoteUseCase
{
    Task<Result<QuoteResponse?>> ExecuteAsync(Guid id, UpdateQuoteRequest request, CancellationToken cancellationToken = default);
}
