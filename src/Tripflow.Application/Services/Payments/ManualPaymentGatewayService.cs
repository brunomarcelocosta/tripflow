using Tripflow.Application.Services.Payments;

namespace Tripflow.Application.Services.Payments;

public class ManualPaymentGatewayService : IPaymentGatewayService
{
    public Task<CreatePaymentLinkResult> CreatePaymentLinkAsync(CreatePaymentLinkCommand command, CancellationToken cancellationToken = default)
    {
        var token = Guid.NewGuid().ToString("N");
        var externalId = $"manual-{command.PaymentId:N}-{token[..8]}";
        var url = $"https://pay.tripflow.local/{command.PaymentId}/{token}";
        return Task.FromResult(new CreatePaymentLinkResult(externalId, url));
    }
}
