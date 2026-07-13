using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tripflow.Application.Options;

namespace Tripflow.Application.Services.Payments.InfinitePay;

public sealed class InfinitePayCheckoutService(
    IHttpClientFactory httpClientFactory,
    IOptions<InfinitePayOptions> options,
    ILogger<InfinitePayCheckoutService> logger) : IInfinitePayCheckoutService
{
    public Task<InfinitePayCheckoutResult> CreatePlatformCheckoutAsync(CreateInfinitePayPlatformCheckoutCommand command, CancellationToken cancellationToken = default)
    {
        var handle = NormalizeHandle(options.Value.ApiKey);
        if (string.IsNullOrWhiteSpace(handle))
            throw new InvalidOperationException("Handle InfinitePay da plataforma não configurado.");

        var webhookUrl = BuildWebhookUrl("/api/webhooks/infinitepay/platform");
        var orderNsu = command.ExternalReference ?? command.SubscriptionPlanId.ToString("N");

        return CreateCheckoutAsync(
            handle,
            command.PlanName,
            command.Amount,
            orderNsu,
            command.SuccessUrl,
            webhookUrl,
            command.CustomerName,
            command.CustomerEmail,
            command.CustomerPhone,
            cancellationToken);
    }

    public Task<InfinitePayCheckoutResult> CreateTenantCheckoutAsync(CreateInfinitePayTenantCheckoutCommand command, CancellationToken cancellationToken = default)
    {
        var handle = NormalizeHandle(command.TenantApiKey);
        if (string.IsNullOrWhiteSpace(handle))
            throw new InvalidOperationException("Handle InfinitePay do tenant não configurado.");

        var webhookUrl = command.WebhookUrl ?? BuildWebhookUrl("/api/webhooks/payments/infinitepay");
        var orderNsu = command.PaymentId.ToString("N");

        return CreateCheckoutAsync(
            handle,
            command.Description,
            command.Amount,
            orderNsu,
            command.SuccessUrl,
            webhookUrl,
            command.CustomerName,
            command.CustomerEmail,
            command.CustomerPhone,
            cancellationToken);
    }

    private async Task<InfinitePayCheckoutResult> CreateCheckoutAsync(
        string handle,
        string description,
        decimal amount,
        string orderNsu,
        string redirectUrl,
        string webhookUrl,
        string customerName,
        string customerEmail,
        string? customerPhone,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(nameof(InfinitePayCheckoutService));
        client.BaseAddress = new Uri(options.Value.CheckoutBaseUrl.TrimEnd('/') + "/");

        var request = new InfinitePayCreateLinkRequest(
            Handle: handle,
            Items:
            [
                new InfinitePayItemRequest(
                    Quantity: 1,
                    Price: ToCents(amount),
                    Description: description.Length > 200 ? description[..200] : description)
            ],
            OrderNsu: orderNsu,
            RedirectUrl: redirectUrl,
            WebhookUrl: webhookUrl,
            Customer: new InfinitePayCustomerRequest(
                Name: customerName,
                Email: customerEmail,
                PhoneNumber: NormalizePhone(customerPhone)));

        using var response = await client.PostAsJsonAsync("links", request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                "InfinitePayCheckoutService | Falha HTTP {StatusCode} | OrderNsu={OrderNsu}",
                (int)response.StatusCode,
                orderNsu);
            throw new InvalidOperationException("Falha ao criar checkout na InfinitePay.");
        }

        var parsed = ParseCreateLinkResponse(body);
        if (parsed is null)
        {
            logger.LogError("InfinitePayCheckoutService | Resposta inválida | OrderNsu={OrderNsu}", orderNsu);
            throw new InvalidOperationException("Resposta inválida da InfinitePay ao criar checkout.");
        }

        return new InfinitePayCheckoutResult(parsed.Value.ExternalCheckoutId, parsed.Value.CheckoutUrl, body);
    }

    private string BuildWebhookUrl(string path)
        => $"{options.Value.ApiPublicBaseUrl.TrimEnd('/')}{path}";

    private static string NormalizeHandle(string? handle)
    {
        if (string.IsNullOrWhiteSpace(handle))
            return string.Empty;

        return handle.Trim().TrimStart('$');
    }

    private static string? NormalizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return null;

        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (digits.Length == 0)
            return null;

        return digits.StartsWith("55", StringComparison.Ordinal) ? $"+{digits}" : $"+55{digits}";
    }

    private static int ToCents(decimal amount)
        => (int)Math.Round(amount * 100m, MidpointRounding.AwayFromZero);

    private static (string ExternalCheckoutId, string CheckoutUrl)? ParseCreateLinkResponse(string body)
    {
        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        var url = GetString(root, "link", "url", "checkout_url");
        var slug = GetString(root, "slug", "invoice_slug", "id");

        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(slug))
            return null;

        return (slug, url);
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

    private sealed record InfinitePayCreateLinkRequest(
        [property: JsonPropertyName("handle")] string Handle,
        [property: JsonPropertyName("items")] InfinitePayItemRequest[] Items,
        [property: JsonPropertyName("order_nsu")] string OrderNsu,
        [property: JsonPropertyName("redirect_url")] string RedirectUrl,
        [property: JsonPropertyName("webhook_url")] string WebhookUrl,
        [property: JsonPropertyName("customer")] InfinitePayCustomerRequest Customer);

    private sealed record InfinitePayItemRequest(
        [property: JsonPropertyName("quantity")] int Quantity,
        [property: JsonPropertyName("price")] int Price,
        [property: JsonPropertyName("description")] string Description);

    private sealed record InfinitePayCustomerRequest(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("phone_number")] string? PhoneNumber);
}
