using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Subscriptions;

namespace Tripflow.Infra.Data.Mappings.Subscriptions;

public sealed class PlanFeatureMap : BaseEntityMap<PlanFeature>
{
    public override void Configure(EntityTypeBuilder<PlanFeature> builder)
    {
        base.Configure(builder);

        builder.ToTable("plan_features");

        builder.Property(x => x.FeatureCode)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LimitValue);
        builder.Property(x => x.Enabled).IsRequired();

        builder.HasOne(x => x.SubscriptionPlan)
            .WithMany(x => x.Features)
            .HasForeignKey(x => x.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.SubscriptionPlanId, x.FeatureCode })
            .IsUnique();
    }
}
