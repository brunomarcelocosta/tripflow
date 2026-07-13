using Tripflow.Application.DTOs.Common;

namespace Tripflow.Application.UseCases.Quotes.Interfaces;

public interface IDeleteQuoteUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}
