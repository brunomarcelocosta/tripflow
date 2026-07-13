using System.Text.Json;
using Tripflow.Domain.Enums;

namespace Tripflow.Application.Services.Payments;

public class InfinitePayPaymentWebhookParser : IPaymentWebhookParser
{
    public Task<ParsedPaymentWebhook?> ParseAsync(string providerCode, string payload, CancellationToken cancellationToken = default)
    {
        if (!string.Equals(providerCode, "infinitepay", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult<ParsedPaymentWebhook?>(null);

        try
        {
            using var doc = JsonDocument.Parse(payload);
            var root = doc.RootElement;

            var transactionNsu = GetString(root, "transaction_nsu");
            var orderNsu = GetString(root, "order_nsu");
            var paidAmount = GetDecimal(root, "paid_amount", "amount");

            if (string.IsNullOrWhiteSpace(transactionNsu))
                return Task.FromResult<ParsedPaymentWebhook?>(null);

            return Task.FromResult<ParsedPaymentWebhook?>(new ParsedPaymentWebhook(
                transactionNsu,
                orderNsu,
                PaymentStatus.Paid,
                paidAmount.HasValue ? paidAmount.Value / 100m : null,
                null,
                null,
                DateTime.UtcNow));
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
