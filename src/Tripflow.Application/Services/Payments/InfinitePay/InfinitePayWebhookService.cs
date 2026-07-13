using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tripflow.Application.Options;

namespace Tripflow.Application.Services.Payments.InfinitePay;

public sealed class InfinitePayWebhookService(
    IOptions<InfinitePayOptions> options,
    ILogger<InfinitePayWebhookService> logger) : IInfinitePayWebhookService
{
    public Task<InfinitePayWebhookParsedEvent?> ParseAsync(string payload, IReadOnlyDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        try
        {
            using var doc = JsonDocument.Parse(payload);
            var root = doc.RootElement;

            var transactionNsu = GetString(root, "transaction_nsu");
            var invoiceSlug = GetString(root, "invoice_slug", "slug");
            var orderNsu = GetString(root, "order_nsu");
            var paidAmount = GetDecimal(root, "paid_amount", "amount");

            if (string.IsNullOrWhiteSpace(transactionNsu))
                return Task.FromResult<InfinitePayWebhookParsedEvent?>(null);

            var parsed = new InfinitePayWebhookParsedEvent(
                ProviderCode: options.Value.ProviderCode,
                ExternalEventId: transactionNsu,
                ExternalCheckoutId: invoiceSlug,
                ExternalPaymentId: orderNsu,
                Status: "paid",
                Amount: paidAmount.HasValue ? paidAmount.Value / 100m : null,
                PaidAtUtc: DateTime.UtcNow,
                RawPayload: payload);

            return Task.FromResult<InfinitePayWebhookParsedEvent?>(parsed);
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "InfinitePayWebhookService | Payload inválido.");
            return Task.FromResult<InfinitePayWebhookParsedEvent?>(null);
        }
    }

    public Task<bool> ValidateSignatureAsync(string payload, IReadOnlyDictionary<string, string> headers, string? webhookSecret, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(webhookSecret))
            return Task.FromResult(true);

        if (headers.TryGetValue("X-InfinitePay-Signature", out var signature) ||
            headers.TryGetValue("x-infinitepay-signature", out signature))
        {
            logger.LogWarning("InfinitePayWebhookService | Assinatura recebida, validação customizada pendente.");
        }

        return Task.FromResult(true);
    }

    private static string? GetString(JsonElement root, params string[] names)
    {
        foreach (var name in names)
        {
            if (root.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String)
                return prop.GetString();
        }

        return null;
    }

    private static decimal? GetDecimal(JsonElement root, params string[] names)
    {
        foreach (var name in names)
        {
            if (!root.TryGetProperty(name, out var prop))
                continue;

            if (prop.ValueKind == JsonValueKind.Number && prop.TryGetDecimal(out var value))
                return value;

            if (prop.ValueKind == JsonValueKind.String && decimal.TryParse(prop.GetString(), out var parsed))
                return parsed;
        }

        return null;
    }
}
