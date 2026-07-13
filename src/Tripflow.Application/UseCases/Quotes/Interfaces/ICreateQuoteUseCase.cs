using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.DTOs.Responses.Quotes;

namespace Tripflow.Application.UseCases.Quotes.Interfaces;

public interface ICreateQuoteUseCase
{
    Task<Result<QuoteResponse?>> ExecuteAsync(CreateQuoteRequest request, CancellationToken cancellationToken = default);
}
