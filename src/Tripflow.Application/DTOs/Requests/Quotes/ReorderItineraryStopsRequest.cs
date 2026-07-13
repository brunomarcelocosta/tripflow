namespace Tripflow.Application.DTOs.Requests.Quotes;

public sealed record ReorderItineraryStopsRequest(
    IEnumerable<ReorderItineraryStopItem> Stops);

public sealed record ReorderItineraryStopItem(Guid StopId, int Sequence);
