using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities;
using Tripflow.Domain.Entities.Audit;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Entities.Proposals;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Entities.Admin;
using Tripflow.Domain.Entities.Platform;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;
using Tripflow.Infra.Utils;

namespace Tripflow.Infra.Data.Contexts;

public sealed class TripflowDbContext(DbContextOptions<TripflowDbContext> options,
                                      ITenantContext? tenantContext = null) : DbContext(options)
{
    private readonly ITenantContext? _tenantContext = tenantContext;

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantBranding> TenantBrandings => Set<TenantBranding>();
    public DbSet<TenantCommercialSettings> TenantCommercialSettings => Set<TenantCommercialSettings>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserInvitation> UserInvitations => Set<UserInvitation>();

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Traveler> Travelers => Set<Traveler>();
    public DbSet<CustomerPreference> CustomerPreferences => Set<CustomerPreference>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<QuoteItem> QuoteItems => Set<QuoteItem>();
    public DbSet<Itinerary> Itineraries => Set<Itinerary>();
    public DbSet<ItineraryStop> ItineraryStops => Set<ItineraryStop>();
    public DbSet<QuoteFlightItem> QuoteFlightItems => Set<QuoteFlightItem>();
    public DbSet<FlightSegment> FlightSegments => Set<FlightSegment>();
    public DbSet<PaymentFeeRule> PaymentFeeRules => Set<PaymentFeeRule>();
    public DbSet<QuotePricingOption> QuotePricingOptions => Set<QuotePricingOption>();
    public DbSet<QuotePaymentCondition> QuotePaymentConditions => Set<QuotePaymentCondition>();
    public DbSet<Proposal> Proposals => Set<Proposal>();
    public DbSet<ProposalVersion> ProposalVersions => Set<ProposalVersion>();
    public DbSet<ProposalEvent> ProposalEvents => Set<ProposalEvent>();
    public DbSet<PaymentProvider> PaymentProviders => Set<PaymentProvider>();
    public DbSet<TenantPaymentProvider> TenantPaymentProviders => Set<TenantPaymentProvider>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentLink> PaymentLinks => Set<PaymentLink>();
    public DbSet<PaymentWebhookEvent> PaymentWebhookEvents => Set<PaymentWebhookEvent>();
    public DbSet<FinancialTransaction> FinancialTransactions => Set<FinancialTransaction>();
    public DbSet<LoyaltyProgram> LoyaltyPrograms => Set<LoyaltyProgram>();
    public DbSet<CustomerLoyaltyAccount> CustomerLoyaltyAccounts => Set<CustomerLoyaltyAccount>();
    public DbSet<MilesExpirationBatch> MilesExpirationBatches => Set<MilesExpirationBatch>();
    public DbSet<MilesTransaction> MilesTransactions => Set<MilesTransaction>();
    public DbSet<MilesQuoteOption> MilesQuoteOptions => Set<MilesQuoteOption>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<PlanFeature> PlanFeatures => Set<PlanFeature>();
    public DbSet<TenantSubscription> TenantSubscriptions => Set<TenantSubscription>();
    public DbSet<TenantUsage> TenantUsages => Set<TenantUsage>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<PlatformCheckoutSession> PlatformCheckoutSessions => Set<PlatformCheckoutSession>();
    public DbSet<PlatformPaymentEvent> PlatformPaymentEvents => Set<PlatformPaymentEvent>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SupportSession> SupportSessions => Set<SupportSession>();

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ValidateTenantEntities();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ValidateTenantEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ValidateTenantEntities();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new UtcDateTimeConverter());
                }
            }
        }

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TripflowDbContext).Assembly);

        TripflowDbSeedData.Apply(modelBuilder);

        ApplyGlobalFilters(modelBuilder);
    }

    private void ApplyGlobalFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (clrType is null)
                continue;

            var isAuditable = typeof(AuditableEntity).IsAssignableFrom(clrType);
            var isTenantEntity = typeof(ITenantEntity).IsAssignableFrom(clrType);

            if (!isAuditable && !isTenantEntity)
                continue;

            if (isAuditable && isTenantEntity)
            {
                InvokeFilterConfiguration(nameof(ConfigureAuditableTenantFilter), clrType, modelBuilder);
            }
            else if (isAuditable)
            {
                InvokeFilterConfiguration(nameof(ConfigureAuditableFilter), clrType, modelBuilder);
            }
            else
            {
                InvokeFilterConfiguration(nameof(ConfigureTenantFilter), clrType, modelBuilder);
            }
        }
    }

    private void InvokeFilterConfiguration(string methodName, Type clrType, ModelBuilder modelBuilder)
    {
        var entityBuilder = typeof(ModelBuilder)
            .GetMethods()
            .Single(method =>
                method.Name == nameof(ModelBuilder.Entity) &&
                method.IsGenericMethodDefinition &&
                method.GetParameters().Length == 0)
            .MakeGenericMethod(clrType)
            .Invoke(modelBuilder, null);

        var method = typeof(TripflowDbContext)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(clrType);

        method.Invoke(this, [entityBuilder!]);
    }

    private bool IgnoreTenantFilter => _tenantContext is null;

    private bool HasCurrentTenant => _tenantContext is { HasTenant: true };

    private Guid CurrentTenantId => _tenantContext is { HasTenant: true, TenantId: { } tenantId }
        ? tenantId
        : Guid.Empty;

    private void ConfigureAuditableTenantFilter<T>(EntityTypeBuilder<T> builder)
        where T : AuditableEntity, ITenantEntity
    {
        builder.HasQueryFilter(entity =>
            !entity.IsDeleted &&
            (IgnoreTenantFilter || (HasCurrentTenant && entity.TenantId == CurrentTenantId)));
    }

    private void ConfigureAuditableFilter<T>(EntityTypeBuilder<T> builder)
        where T : AuditableEntity
    {
        builder.HasQueryFilter(entity => !entity.IsDeleted);
    }

    private void ConfigureTenantFilter<T>(EntityTypeBuilder<T> builder)
        where T : class, ITenantEntity
    {
        builder.HasQueryFilter(entity =>
            IgnoreTenantFilter || (HasCurrentTenant && entity.TenantId == CurrentTenantId));
    }

    private void ValidateTenantEntities()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(entry =>
                entry.Entity is ITenantEntity &&
                entry.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            var tenantEntity = (ITenantEntity)entry.Entity;

            if (tenantEntity.TenantId == Guid.Empty)
            {
                throw new InvalidOperationException("TenantId is required for tenant-scoped entities.");
            }

            if (_tenantContext is not null &&
                _tenantContext.HasTenant &&
                tenantEntity.TenantId != _tenantContext.TenantId)
            {
                throw new InvalidOperationException("Entity TenantId does not match current TenantContext.");
            }
        }
    }
}
