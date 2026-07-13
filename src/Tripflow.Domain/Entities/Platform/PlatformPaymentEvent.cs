namespace Tripflow.Domain.Entities.Platform;

public sealed class PlatformPaymentEvent : BaseEntity
{
    private PlatformPaymentEvent() { }

    public PlatformPaymentEvent(
        string providerCode,
        string externalEventId,
        string? externalCheckoutId,
        string payload)
    {
        ProviderCode = providerCode;
        ExternalEventId = externalEventId;
        ExternalCheckoutId = externalCheckoutId;
        Payload = payload;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public string ProviderCode { get; private set; } = default!;
    public string ExternalEventId { get; private set; } = default!;
    public string? ExternalCheckoutId { get; private set; }
    public string Payload { get; private set; } = default!;
    public bool Processed { get; private set; }
    public DateTime? ProcessedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public void MarkAsProcessed()
    {
        Processed = true;
        ProcessedAtUtc = DateTime.UtcNow;
    }
}
