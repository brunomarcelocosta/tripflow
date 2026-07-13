using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.DTOs.Responses.Miles;

namespace Tripflow.Application.UseCases.Miles.Interfaces;

public interface IGetMilesQuoteOptionsUseCase
{
    Task<Result<IEnumerable<MilesQuoteOptionResponse>>> ExecuteAsync(Guid quoteId, CancellationToken cancellationToken = default);
}

public interface IGetMilesQuoteOptionByIdUseCase
{
    Task<Result<MilesQuoteOptionResponse?>> ExecuteAsync(Guid quoteId, Guid milesOptionId, CancellationToken cancellationToken = default);
}

public interface ICreateMilesQuoteOptionUseCase
{
    Task<Result<MilesQuoteOptionResponse?>> ExecuteAsync(Guid quoteId, CreateMilesQuoteOptionRequest request, CancellationToken cancellationToken = default);
}

public interface IUpdateMilesQuoteOptionUseCase
{
    Task<Result<MilesQuoteOptionResponse?>> ExecuteAsync(Guid quoteId, Guid milesOptionId, UpdateMilesQuoteOptionRequest request, CancellationToken cancellationToken = default);
}

public interface IDeleteMilesQuoteOptionUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid quoteId, Guid milesOptionId, CancellationToken cancellationToken = default);
}

public interface ISelectMilesQuoteOptionUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid quoteId, Guid milesOptionId, CancellationToken cancellationToken = default);
}
