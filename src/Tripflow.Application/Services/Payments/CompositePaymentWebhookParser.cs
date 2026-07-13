namespace Tripflow.Application.Services.Payments;

public class CompositePaymentWebhookParser(
    InfinitePayPaymentWebhookParser infinitePayParser,
    GenericPaymentWebhookParser genericParser) : IPaymentWebhookParser
{
    public async Task<ParsedPaymentWebhook?> ParseAsync(string providerCode, string payload, CancellationToken cancellationToken = default)
    {
        var infinitePay = await infinitePayParser.ParseAsync(providerCode, payload, cancellationToken);
        if (infinitePay is not null)
            return infinitePay;

        return await genericParser.ParseAsync(providerCode, payload, cancellationToken);
    }
}
