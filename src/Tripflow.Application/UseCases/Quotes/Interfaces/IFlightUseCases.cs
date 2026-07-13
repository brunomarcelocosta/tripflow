using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Quotes;
using Tripflow.Application.DTOs.Responses.Quotes;

namespace Tripflow.Application.UseCases.Quotes.Interfaces;

public interface IGetQuoteFlightsUseCase
{
    Task<Result<IEnumerable<QuoteFlightItemResponse>>> ExecuteAsync(Guid quoteId, CancellationToken cancellationToken = default);
}

public interface IGetQuoteFlightByIdUseCase
{
    Task<Result<QuoteFlightItemResponse?>> ExecuteAsync(Guid quoteId, Guid flightItemId, CancellationToken cancellationToken = default);
}

public interface ICreateQuoteFlightItemUseCase
{
    Task<Result<QuoteFlightItemResponse?>> ExecuteAsync(Guid quoteId, CreateQuoteFlightItemRequest request, CancellationToken cancellationToken = default);
}

public interface IUpdateQuoteFlightItemUseCase
{
    Task<Result<QuoteFlightItemResponse?>> ExecuteAsync(Guid quoteId, Guid flightItemId, UpdateQuoteFlightItemRequest request, CancellationToken cancellationToken = default);
}

public interface IDeleteQuoteFlightItemUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid quoteId, Guid flightItemId, CancellationToken cancellationToken = default);
}

public interface ICreateFlightSegmentUseCase
{
    Task<Result<FlightSegmentResponse?>> ExecuteAsync(Guid quoteId, Guid flightItemId, CreateFlightSegmentRequest request, CancellationToken cancellationToken = default);
}

public interface IUpdateFlightSegmentUseCase
{
    Task<Result<FlightSegmentResponse?>> ExecuteAsync(Guid quoteId, Guid flightItemId, Guid segmentId, UpdateFlightSegmentRequest request, CancellationToken cancellationToken = default);
}

public interface IDeleteFlightSegmentUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid quoteId, Guid flightItemId, Guid segmentId, CancellationToken cancellationToken = default);
}

public interface IReorderFlightSegmentsUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid quoteId, Guid flightItemId, ReorderFlightSegmentsRequest request, CancellationToken cancellationToken = default);
}
