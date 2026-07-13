namespace Tripflow.Application.Options;

public sealed class PaymentGatewayOptions
{
    public const string SectionName = "PaymentGateway";

    public AsaasGatewayOptions Asaas { get; init; } = new();
}

public sealed class AsaasGatewayOptions
{
    public bool Enabled { get; init; } = true;

    public string BaseUrl { get; init; } = "https://api-sandbox.asaas.com/v3";
}
