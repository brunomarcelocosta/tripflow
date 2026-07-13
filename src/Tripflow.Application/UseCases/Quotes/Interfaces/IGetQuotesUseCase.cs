using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Quotes;

namespace Tripflow.Application.UseCases.Quotes.Interfaces;

public interface IGetQuotesUseCase
{
    Task<Result<PagedResponse<QuoteResponse>>> ExecuteAsync(QuoteFilterRequest request, CancellationToken cancellationToken = default);
}
