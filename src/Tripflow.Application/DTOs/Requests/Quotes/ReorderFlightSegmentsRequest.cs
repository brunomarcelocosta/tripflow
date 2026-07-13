namespace Tripflow.Application.DTOs.Requests.Quotes;

public sealed record ReorderFlightSegmentsRequest(
    IEnumerable<ReorderFlightSegmentItem> Segments);

public sealed record ReorderFlightSegmentItem(Guid SegmentId, int Sequence);
