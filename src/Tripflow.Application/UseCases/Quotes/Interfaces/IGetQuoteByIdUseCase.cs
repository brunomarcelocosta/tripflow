using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Quotes;

namespace Tripflow.Application.UseCases.Quotes.Interfaces;

public interface IGetQuoteByIdUseCase
{
    Task<Result<QuoteResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}
