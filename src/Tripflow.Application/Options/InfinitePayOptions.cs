namespace Tripflow.Application.Options;

public sealed class InfinitePayOptions
{
    public const string SectionName = "Payments:InfinitePay";

    public string CheckoutBaseUrl { get; set; } = "https://api.checkout.infinitepay.io";

    public string ApiKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public string WebhookSecret { get; set; } = string.Empty;

    public string CheckoutSuccessUrl { get; set; } = string.Empty;

    public string CheckoutCancelUrl { get; set; } = string.Empty;

    public string PlatformStatementDescriptor { get; set; } = "TripFlow";

    public string PlatformDefaultCurrency { get; set; } = "BRL";

    public string ApiPublicBaseUrl { get; set; } = "http://localhost:5044";

    public string ProviderCode { get; set; } = "infinitepay";
}
