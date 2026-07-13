using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Subscriptions;

namespace Tripflow.Infra.Data.Mappings.Subscriptions;

public sealed class TenantSubscriptionMap : AuditableEntityMap<TenantSubscription>
{
    public override void Configure(EntityTypeBuilder<TenantSubscription> builder)
    {
        base.Configure(builder);

        builder.ToTable("tenant_subscriptions");

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.StartedAtUtc).IsRequired();
        builder.Property(x => x.ExpiresAtUtc);
        builder.Property(x => x.TrialEndsAtUtc);
        builder.Property(x => x.CancelledAtUtc);

        builder.HasOne(x => x.Tenant)
            .WithOne(x => x.CurrentSubscription)
            .HasForeignKey<TenantSubscription>(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SubscriptionPlan)
            .WithMany()
            .HasForeignKey(x => x.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TenantId)
            .IsUnique();

        builder.HasIndex(x => x.SubscriptionPlanId);
        builder.HasIndex(x => x.Status);
    }
}
