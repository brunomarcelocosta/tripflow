using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Admin;
using Tripflow.Infra.Data.Contexts;
using Tripflow.Infra.Data.Repositories;
using Tripflow.Infra.Data.Repositories.Admin;
using Tripflow.Infra.Data.Repositories.Audit;
using Tripflow.Infra.Data.Repositories.CRM;
using Tripflow.Infra.Data.Repositories.Identity;
using Tripflow.Infra.Data.Repositories.Miles;
using Tripflow.Infra.Data.Repositories.Payments;
using Tripflow.Infra.Data.Repositories.Pricing;
using Tripflow.Infra.Data.Repositories.Platform;
using Tripflow.Infra.Data.Repositories.Proposals;
using Tripflow.Infra.Data.Repositories.Quotes;
using Tripflow.Infra.Data.Repositories.Subscriptions;
using Tripflow.Infra.Data.Repositories.Tenants;

namespace Tripflow.Infra.IoC.DI;

public static class DatabaseDI
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<TripflowDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddRepositories();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ITenantBrandingRepository, TenantBrandingRepository>();
        services.AddScoped<ITenantCommercialSettingsRepository, TenantCommercialSettingsRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserInvitationRepository, UserInvitationRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ITravelerRepository, TravelerRepository>();
        services.AddScoped<ICustomerPreferenceRepository, CustomerPreferenceRepository>();
        services.AddScoped<IQuoteRepository, QuoteRepository>();
        services.AddScoped<IQuoteItemRepository, QuoteItemRepository>();
        services.AddScoped<IItineraryRepository, ItineraryRepository>();
        services.AddScoped<IItineraryStopRepository, ItineraryStopRepository>();
        services.AddScoped<IQuoteFlightItemRepository, QuoteFlightItemRepository>();
        services.AddScoped<IFlightSegmentRepository, FlightSegmentRepository>();
        services.AddScoped<IPaymentFeeRuleRepository, PaymentFeeRuleRepository>();
        services.AddScoped<IQuotePricingOptionRepository, QuotePricingOptionRepository>();
        services.AddScoped<IQuotePaymentConditionRepository, QuotePaymentConditionRepository>();
        services.AddScoped<IProposalRepository, ProposalRepository>();
        services.AddScoped<IProposalVersionRepository, ProposalVersionRepository>();
        services.AddScoped<IProposalEventRepository, ProposalEventRepository>();
        services.AddScoped<IPaymentProviderRepository, PaymentProviderRepository>();
        services.AddScoped<ITenantPaymentProviderRepository, TenantPaymentProviderRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPaymentLinkRepository, PaymentLinkRepository>();
        services.AddScoped<IPaymentWebhookEventRepository, PaymentWebhookEventRepository>();
        services.AddScoped<IFinancialTransactionRepository, FinancialTransactionRepository>();
        services.AddScoped<ILoyaltyProgramRepository, LoyaltyProgramRepository>();
        services.AddScoped<ICustomerLoyaltyAccountRepository, CustomerLoyaltyAccountRepository>();
        services.AddScoped<IMilesExpirationBatchRepository, MilesExpirationBatchRepository>();
        services.AddScoped<IMilesTransactionRepository, MilesTransactionRepository>();
        services.AddScoped<IMilesQuoteOptionRepository, MilesQuoteOptionRepository>();
        services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
        services.AddScoped<IPlanFeatureRepository, PlanFeatureRepository>();
        services.AddScoped<ITenantSubscriptionRepository, TenantSubscriptionRepository>();
        services.AddScoped<ITenantUsageRepository, TenantUsageRepository>();
        services.AddScoped<ILeadRepository, LeadRepository>();
        services.AddScoped<IPlatformCheckoutSessionRepository, PlatformCheckoutSessionRepository>();
        services.AddScoped<IPlatformPaymentEventRepository, PlatformPaymentEventRepository>();
        services.AddScoped<IPlatformAdminRepository, PlatformAdminRepository>();
        services.AddScoped<ISupportSessionRepository, SupportSessionRepository>();
    }
}
