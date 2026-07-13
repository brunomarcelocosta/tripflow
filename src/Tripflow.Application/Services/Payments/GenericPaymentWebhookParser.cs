using System.Text.Json;
using Tripflow.Domain.Enums;

namespace Tripflow.Application.Services.Payments;

public class GenericPaymentWebhookParser : IPaymentWebhookParser
{
    public Task<ParsedPaymentWebhook?> ParseAsync(string providerCode, string payload, CancellationToken cancellationToken = default)
    {
        try
        {
            using var doc = JsonDocument.Parse(payload);
            var root = doc.RootElement;

            var eventId = GetString(root, "externalEventId", "eventId", "id");
            if (string.IsNullOrWhiteSpace(eventId))
                return Task.FromResult<ParsedPaymentWebhook?>(null);

            var paymentId = GetString(root, "externalPaymentId", "paymentId");
            var statusStr = GetString(root, "status", "paymentStatus");
            PaymentStatus? status = null;
            if (!string.IsNullOrWhiteSpace(statusStr) && Enum.TryParse<PaymentStatus>(statusStr, true, out var parsed))
                status = parsed;

            return Task.FromResult<ParsedPaymentWebhook?>(new ParsedPaymentWebhook(
                eventId,
                paymentId,
                status,
                GetDecimal(root, "grossAmount", "amount"),
                GetDecimal(root, "feeAmount"),
                GetDecimal(root, "netAmount"),
                GetDateTime(root, "paidAtUtc", "paidAt")));
        }
        catch (JsonException)
        {
            return Task.FromResult<ParsedPaymentWebhook?>(null);
        }
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
            if (root.TryGetProperty(name, out var prop) && prop.TryGetDecimal(out var value))
                return value;
        }
        return null;
    }

    private static DateTime? GetDateTime(JsonElement root, params string[] names)
    {
        foreach (var name in names)
        {
            if (root.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String &&
                DateTime.TryParse(prop.GetString(), out var dt))
                return dt.ToUniversalTime();
        }
        return null;
    }
}
