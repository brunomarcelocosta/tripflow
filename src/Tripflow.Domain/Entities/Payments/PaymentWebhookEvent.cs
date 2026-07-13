namespace Tripflow.Domain.Entities.Payments;

public sealed class PaymentWebhookEvent : BaseEntity
{
    private PaymentWebhookEvent() { }

    public PaymentWebhookEvent(string providerCode, string externalEventId, string? externalPaymentId, string payload)
    {
        ProviderCode = providerCode;
        ExternalEventId = externalEventId;
        ExternalPaymentId = externalPaymentId;
        Payload = payload;
        Processed = false;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public string ProviderCode { get; private set; } = default!;
    public string ExternalEventId { get; private set; } = default!;
    public string? ExternalPaymentId { get; private set; }
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

