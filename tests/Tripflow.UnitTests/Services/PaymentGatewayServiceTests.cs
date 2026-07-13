using Moq;
using Tripflow.Application.Options;
using Tripflow.Application.Services.Payments;
using Tripflow.Application.Services.Payments.InfinitePay;
using Tripflow.Domain.Enums;

namespace Tripflow.UnitTests.Services;

public class PaymentGatewayServiceTests
{
    [Fact]
    public async Task CreatePaymentLinkAsync_Should_UseManualGateway_When_ProviderIsManual()
    {
        var manual = new ManualPaymentGatewayService();
        var asaas = new AsaasPaymentGatewayService(
            new StubHttpClientFactory(),
            Microsoft.Extensions.Options.Options.Create(new PaymentGatewayOptions()),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<AsaasPaymentGatewayService>.Instance);
        var infinitePay = new InfinitePayPaymentGatewayService(
            Mock.Of<IInfinitePayCheckoutService>(),
            Microsoft.Extensions.Options.Options.Create(new InfinitePayOptions()),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<InfinitePayPaymentGatewayService>.Instance);
        var service = new PaymentGatewayService(
            manual,
            asaas,
            infinitePay,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<PaymentGatewayService>.Instance);

        var command = new CreatePaymentLinkCommand(
            Guid.NewGuid(), Guid.NewGuid(), "manual", null, 100m, PaymentMethod.Pix, 1, null);

        var result = await service.CreatePaymentLinkAsync(command);

        Assert.StartsWith("manual-", result.ExternalLinkId, StringComparison.Ordinal);
        Assert.Contains(command.PaymentId.ToString(), result.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreatePaymentLinkAsync_Should_Throw_When_AsaasWithoutApiKey()
    {
        var manual = new ManualPaymentGatewayService();
        var asaas = new AsaasPaymentGatewayService(
            new StubHttpClientFactory(),
            Microsoft.Extensions.Options.Options.Create(new PaymentGatewayOptions()),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<AsaasPaymentGatewayService>.Instance);
        var infinitePay = new InfinitePayPaymentGatewayService(
            Mock.Of<IInfinitePayCheckoutService>(),
            Microsoft.Extensions.Options.Options.Create(new InfinitePayOptions()),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<InfinitePayPaymentGatewayService>.Instance);
        var service = new PaymentGatewayService(
            manual,
            asaas,
            infinitePay,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<PaymentGatewayService>.Instance);

        var command = new CreatePaymentLinkCommand(
            Guid.NewGuid(), Guid.NewGuid(), "asaas", null, 100m, PaymentMethod.Pix, 1, null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreatePaymentLinkAsync(command));
    }

    private sealed class StubHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(new HttpClientHandler());
    }
}
