using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Subscriptions;

namespace Tripflow.Infra.Data.Mappings.Subscriptions;

public sealed class SubscriptionPlanMap : AuditableEntityMap<SubscriptionPlan>
{
    public override void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        base.Configure(builder);

        builder.ToTable("subscription_plans");

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.MonthlyPrice).HasPrecision(18, 2);
        builder.Property(x => x.AnnualPrice).HasPrecision(18, 2);
        builder.Property(x => x.MaxUsers);
        builder.Property(x => x.MaxQuotesPerMonth);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasMany(x => x.Features)
            .WithOne(x => x.SubscriptionPlan)
            .HasForeignKey(x => x.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasIndex(x => x.Status);
    }
}
