using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tripflow.Application.Options;
using Tripflow.Domain.Enums;

namespace Tripflow.Application.Services.Payments;

public class AsaasPaymentGatewayService(
    IHttpClientFactory httpClientFactory,
    IOptions<PaymentGatewayOptions> options,
    ILogger<AsaasPaymentGatewayService> logger) : IPaymentGatewayService
{
    public async Task<CreatePaymentLinkResult> CreatePaymentLinkAsync(CreatePaymentLinkCommand command, CancellationToken cancellationToken = default)
    {
        if (!options.Value.Asaas.Enabled)
            throw new InvalidOperationException("Gateway Asaas desabilitado na configuração.");

        if (string.IsNullOrWhiteSpace(command.ApiKey))
            throw new InvalidOperationException("ApiKey do Asaas não configurada para o tenant.");

        var client = httpClientFactory.CreateClient(nameof(AsaasPaymentGatewayService));
        client.BaseAddress = new Uri(options.Value.Asaas.BaseUrl.TrimEnd('/') + "/");
        client.DefaultRequestHeaders.Remove("access_token");
        client.DefaultRequestHeaders.Add("access_token", command.ApiKey);
        client.DefaultRequestHeaders.Add("User-Agent", "Tripflow-Backend/1.0");

        var request = new AsaasPaymentLinkRequest(
            Name: $"Pagamento {command.PaymentId:N}",
            Description: $"Pagamento TripFlow #{command.PaymentId:N}",
            Value: command.GrossAmount,
            BillingType: MapBillingType(command.PaymentMethod),
            ChargeType: "DETACHED",
            DueDateLimitDays: command.ExpiresAtUtc.HasValue
                ? Math.Max(1, (int)Math.Ceiling((command.ExpiresAtUtc.Value - DateTime.UtcNow).TotalDays))
                : 7);

        using var response = await client.PostAsJsonAsync("paymentLinks", request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("AsaasPaymentGatewayService | Falha HTTP {StatusCode} | {Body}", (int)response.StatusCode, body);
            throw new InvalidOperationException("Falha ao criar link de pagamento no Asaas.");
        }

        var parsed = System.Text.Json.JsonSerializer.Deserialize<AsaasPaymentLinkResponse>(body);
        if (parsed is null || string.IsNullOrWhiteSpace(parsed.Id) || string.IsNullOrWhiteSpace(parsed.Url))
            throw new InvalidOperationException("Resposta inválida do Asaas ao criar link de pagamento.");

        return new CreatePaymentLinkResult(parsed.Id, parsed.Url);
    }

    private static string MapBillingType(PaymentMethod method) => method switch
    {
        PaymentMethod.Pix => "PIX",
        PaymentMethod.CreditCard => "CREDIT_CARD",
        PaymentMethod.BankSlip => "BOLETO",
        _ => "UNDEFINED"
    };

    private sealed record AsaasPaymentLinkRequest(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("value")] decimal Value,
        [property: JsonPropertyName("billingType")] string BillingType,
        [property: JsonPropertyName("chargeType")] string ChargeType,
        [property: JsonPropertyName("dueDateLimitDays")] int DueDateLimitDays);

    private sealed record AsaasPaymentLinkResponse(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("url")] string Url);
}
