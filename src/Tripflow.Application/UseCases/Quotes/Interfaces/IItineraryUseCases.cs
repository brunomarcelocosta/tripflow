using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.DTOs.Responses.Quotes;

namespace Tripflow.Application.UseCases.Quotes.Interfaces;

public interface IGetQuoteItineraryUseCase
{
    Task<Result<ItineraryResponse?>> ExecuteAsync(Guid quoteId, CancellationToken cancellationToken = default);
}

public interface IUpdateQuoteItineraryUseCase
{
    Task<Result<ItineraryResponse?>> ExecuteAsync(Guid quoteId, UpdateItineraryRequest request, CancellationToken cancellationToken = default);
}

public interface ICreateItineraryStopUseCase
{
    Task<Result<ItineraryStopResponse?>> ExecuteAsync(Guid quoteId, CreateItineraryStopRequest request, CancellationToken cancellationToken = default);
}

public interface IUpdateItineraryStopUseCase
{
    Task<Result<ItineraryStopResponse?>> ExecuteAsync(Guid quoteId, Guid stopId, UpdateItineraryStopRequest request, CancellationToken cancellationToken = default);
}

public interface IDeleteItineraryStopUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid quoteId, Guid stopId, CancellationToken cancellationToken = default);
}

public interface IReorderItineraryStopsUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid quoteId, ReorderItineraryStopsRequest request, CancellationToken cancellationToken = default);
}
