using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Tripflow.Application.Options;
using Tripflow.Application.Services.Payments;
using Tripflow.Application.Services.Payments.InfinitePay;
using Tripflow.Application.Services.Proposals;
using Tripflow.Application.UseCases.Payments;
using Tripflow.Application.UseCases.Proposals;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Entities.Proposals;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces.Storage;
using Tripflow.Infra.Data.Repositories.Payments;
using Tripflow.Infra.Data.Repositories.Pricing;
using Tripflow.Infra.Data.Repositories.Proposals;
using Tripflow.Infra.Data.Repositories.Quotes;
using Tripflow.IntegrationTests.Utils;

namespace Tripflow.IntegrationTests.Features;

public class ProposalPaymentFlowIntegrationTests
{
    [Fact]
    public async Task FullFlow_QuoteToProposalToPaymentToWebhook_ShouldSucceed()
    {
        var tenantContext = new TestTenantContext();
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();

        Guid tenantId;
        Guid quoteId;
        Guid pricingOptionId;
        Guid proposalId;
        string publicToken;
        Guid paymentId;
        string externalPaymentId;

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var tenant = new Tenant("Agência Demo LTDA", "Agência Demo", null, "demo@tripflow.com", null, TenantStatus.Active, "system");
            seed.Tenants.Add(tenant);
            seed.TenantCommercialSettings.Add(new TenantCommercialSettings(tenant.Id, "system"));
            seed.TenantBrandings.Add(new TenantBranding(tenant.Id, null, "#0055AA", "#FF6600", "#111111", "Rodapé demo", "system"));

            var customer = new Customer(tenant.Id, CustomerType.Person, "Cliente Demo", null, "cliente@demo.com", null, null, null, "system");
            seed.Customers.Add(customer);

            var quote = new Quote(
                tenant.Id, customer.Id, "QT-202605-000001", "Viagem Demo", QuoteType.CompleteItinerary,
                "GRU", "LIS", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(37)),
                2, 0, 0, "Notas demo", DateTime.UtcNow.AddDays(15), null, "system");
            quote.MarkAsCalculated("system");
            seed.Quotes.Add(quote);

            var pricing = new QuotePricingOption(tenant.Id, quote.Id, "Opção PIX", 5000m, 800m, null, null, 4800m, 5200m, true, "system");
            seed.QuotePricingOptions.Add(pricing);

            await seed.SaveChangesAsync();
            tenantId = tenant.Id;
            quoteId = quote.Id;
            pricingOptionId = pricing.Id;
            tenantContext.SetTenant(tenantId);
        }

        await using var context = TripflowDbContextTestFactory.Create(tenantContext, databaseName);

        var quoteRepo = new QuoteRepository(context);
        var proposalRepo = new ProposalRepository(context);
        var pricingRepo = new QuotePricingOptionRepository(context);
        var versionRepo = new ProposalVersionRepository(context);
        var eventRepo = new ProposalEventRepository(context);
        var paymentRepo = new PaymentRepository(context);
        var linkRepo = new PaymentLinkRepository(context);
        var tenantProviderRepo = new TenantPaymentProviderRepository(context);
        var webhookRepo = new PaymentWebhookEventRepository(context);
        var financialRepo = new FinancialTransactionRepository(context);

        var eventService = new ProposalEventService(eventRepo);
        var templateRenderer = new ProposalTemplateRenderer(proposalRepo);
        var pdfGenerator = new ProposalPdfGenerator(Options.Create(new PdfOptions { Enabled = false }), NullLogger<ProposalPdfGenerator>.Instance);
        var documentService = new ProposalDocumentService(proposalRepo, versionRepo, templateRenderer, pdfGenerator, new InMemoryFileStorage(), eventService);
        var financialService = new FinancialTransactionService(financialRepo);
        var infinitePay = new InfinitePayPaymentGatewayService(new StubInfinitePayCheckoutService(), Options.Create(new InfinitePayOptions()), NullLogger<InfinitePayPaymentGatewayService>.Instance);
        var gatewayService = new PaymentGatewayService(new ManualPaymentGatewayService(), new AsaasPaymentGatewayService(new InMemoryHttpClientFactory(), Options.Create(new PaymentGatewayOptions()), NullLogger<AsaasPaymentGatewayService>.Instance), infinitePay, NullLogger<PaymentGatewayService>.Instance);
        var webhookParser = new GenericPaymentWebhookParser();

        publicToken = Guid.NewGuid().ToString("N");
        var proposalNumber = await proposalRepo.GenerateNextProposalNumberAsync(tenantId);
        var proposal = new Proposal(
            tenantId, quoteId, pricingOptionId, proposalNumber,
            ProposalStatus.Draft, publicToken, $"http://localhost/public/proposals/{publicToken}", null,
            DateTime.UtcNow.AddDays(7), "system");
        await proposalRepo.AddAsync(proposal);
        proposalId = proposal.Id;

        var trackedProposal = await proposalRepo.GetTrackedByIdAndTenantAsync(proposalId, tenantId);
        await documentService.GenerateVersionAsync(trackedProposal!, false, null, "system");

        var getPublic = new GetPublicProposalByTokenUseCase(proposalRepo, eventService, NullLogger<GetPublicProposalByTokenUseCase>.Instance);
        var publicView = await getPublic.ExecuteAsync(publicToken, "127.0.0.1", "integration-test");
        Assert.True(publicView.Success, publicView.Error);

        var approve = new ApprovePublicProposalUseCase(proposalRepo, quoteRepo, eventService, NullLogger<ApprovePublicProposalUseCase>.Instance);
        var approveResult = await approve.ExecuteAsync(publicToken, "127.0.0.1", "integration-test");
        Assert.True(approveResult.Success, approveResult.Error);
        Assert.Equal(ProposalStatus.Approved, approveResult.Data!.Status);

        var manualProvider = new TenantPaymentProvider(
            tenantId,
            Guid.Parse("55555555-5555-5555-5555-555555550001"),
            "Manual local",
            null, null, null,
            true,
            PaymentProviderStatus.Active,
            "system");
        await tenantProviderRepo.AddAsync(manualProvider);

        var payment = new Payment(
            tenantId, quoteId, proposalId, manualProvider.Id, null,
            PaymentMethod.Pix, PaymentStatus.Pending, 1, 4800m, 0m, 4800m, null, "system");
        await paymentRepo.AddAsync(payment);
        paymentId = payment.Id;

        var gatewayResult = await gatewayService.CreatePaymentLinkAsync(
            new CreatePaymentLinkCommand(tenantId, paymentId, "manual", null, 4800m, PaymentMethod.Pix, 1, null));
        externalPaymentId = gatewayResult.ExternalLinkId;

        payment = (await paymentRepo.GetTrackedByIdAndTenantAsync(paymentId, tenantId))!;
        payment.SetExternalPaymentId(externalPaymentId, "system");
        await paymentRepo.UpdateAsync(payment);

        await linkRepo.AddAsync(new PaymentLink(
            tenantId, paymentId, gatewayResult.ExternalLinkId, gatewayResult.Url, null, PaymentLinkStatus.Active, "system"));

        await eventService.RegisterAsync(tenantId, proposalId, ProposalEventType.PaymentLinkCreated, "Link de pagamento criado");

        var processWebhook = new ProcessPaymentWebhookUseCase(
            webhookRepo, webhookParser, paymentRepo, proposalRepo, pricingRepo, financialService,
            NullLogger<ProcessPaymentWebhookUseCase>.Instance);
        var receiveWebhook = new ReceivePaymentWebhookUseCase(
            webhookParser, webhookRepo, processWebhook, NullLogger<ReceivePaymentWebhookUseCase>.Instance);

        var payload = JsonSerializer.Serialize(new
        {
            externalEventId = "evt-integration-001",
            externalPaymentId,
            status = nameof(PaymentStatus.Paid),
            grossAmount = 4800m,
            netAmount = 4800m,
            paidAtUtc = DateTime.UtcNow
        });

        var webhookResult = await receiveWebhook.ExecuteAsync("manual", payload);
        Assert.True(webhookResult.Success, webhookResult.Error);

        var paidPayment = await paymentRepo.GetByIdAndTenantAsync(paymentId, tenantId);
        Assert.NotNull(paidPayment);
        Assert.Equal(PaymentStatus.Paid, paidPayment!.Status);
        Assert.True(await financialRepo.ExistsSaleByPaymentIdAsync(paymentId, tenantId));

        var duplicateWebhook = await receiveWebhook.ExecuteAsync("manual", payload);
        Assert.True(duplicateWebhook.Success);

        var versions = await versionRepo.GetByProposalIdAsync(proposalId, tenantId);
        Assert.NotEmpty(versions);
        Assert.NotEmpty(await eventRepo.GetByProposalIdAsync(proposalId, tenantId));
    }

    private sealed class InMemoryFileStorage : IFileStorageService
    {
        public Task<string> UploadAsync(Stream content, string folder, string fileName, string contentType, CancellationToken cancellationToken = default)
            => Task.FromResult($"{folder}/{fileName}");

        public Task DeleteAsync(string fileUrlOrPath, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class InMemoryHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(new HttpClientHandler());
    }

    private sealed class StubInfinitePayCheckoutService : IInfinitePayCheckoutService
    {
        public Task<InfinitePayCheckoutResult> CreatePlatformCheckoutAsync(
            CreateInfinitePayPlatformCheckoutCommand command,
            CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<InfinitePayCheckoutResult> CreateTenantCheckoutAsync(
            CreateInfinitePayTenantCheckoutCommand command,
            CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }
}
