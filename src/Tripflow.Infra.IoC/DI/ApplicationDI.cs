using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Tripflow.Application.Options;
using Tripflow.Application.Services.Audit;
using Tripflow.Application.Services.Miles;
using Tripflow.Application.Services.Payments;
using Tripflow.Application.Services.Payments.InfinitePay;
using Tripflow.Application.Services.Pricing;
using Tripflow.Application.Services.Proposals;
using Tripflow.Application.Services.Subscriptions;
using Tripflow.Application.UseCases.Tenants.Interfaces;

namespace Tripflow.Infra.IoC.DI;

public static class ApplicationDI
{
    private static readonly Type ApplicationAssemblyAnchor = typeof(ICreateTenantUseCase);

    public static void AddUseCases(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        services.Configure<FrontendOptions>(configuration.GetSection(FrontendOptions.SectionName));
        services.Configure<PdfOptions>(configuration.GetSection(PdfOptions.SectionName));
        services.Configure<PaymentGatewayOptions>(configuration.GetSection(PaymentGatewayOptions.SectionName));
        services.Configure<InfinitePayOptions>(configuration.GetSection(InfinitePayOptions.SectionName));
        services.AddValidatorsFromAssembly(ApplicationAssemblyAnchor);
        services.AddUseCasesFromAssembly(ApplicationAssemblyAnchor);
        services.AddApplicationServices();
    }

    private static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IQuotePricingCalculatorService, QuotePricingCalculatorService>();
        services.AddScoped<IProposalEventService, ProposalEventService>();
        services.AddScoped<IProposalTemplateRenderer, ProposalTemplateRenderer>();
        services.AddScoped<IProposalPdfGenerator, ProposalPdfGenerator>();
        services.AddScoped<IProposalDocumentService, ProposalDocumentService>();
        services.AddHttpClient(nameof(AsaasPaymentGatewayService));
        services.AddHttpClient(nameof(InfinitePayCheckoutService));
        services.AddScoped<ManualPaymentGatewayService>();
        services.AddScoped<AsaasPaymentGatewayService>();
        services.AddScoped<InfinitePayPaymentGatewayService>();
        services.AddScoped<IInfinitePayCheckoutService, InfinitePayCheckoutService>();
        services.AddScoped<IInfinitePayWebhookService, InfinitePayWebhookService>();
        services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();
        services.AddScoped<IPaymentWebhookParser, GenericPaymentWebhookParser>();
        services.AddScoped<IFinancialTransactionService, FinancialTransactionService>();
        services.AddScoped<IMilesLedgerService, MilesLedgerService>();
        services.AddScoped<ITenantUsageService, TenantUsageService>();
        services.AddScoped<IAuditService, AuditService>();
    }

    private static void AddValidatorsFromAssembly(this IServiceCollection services, Type assemblyAnchor)
    {
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;

        services.AddValidatorsFromAssemblyContaining(assemblyAnchor);
    }

    private static void AddUseCasesFromAssembly(this IServiceCollection services, Type assemblyAnchor)
    {
        var assembly = assemblyAnchor.Assembly;

        var useCaseTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                && t.Name.EndsWith("UseCase", StringComparison.Ordinal));

        foreach (var implementation in useCaseTypes)
        {
            var serviceType = implementation.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"I{implementation.Name}");

            if (serviceType is null)
                continue;

            services.AddScoped(serviceType, implementation);
        }
    }
}
