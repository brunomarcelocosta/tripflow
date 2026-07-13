using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Platform;

namespace Tripflow.Infra.Data.Mappings.Platform;

public sealed class PlatformCheckoutSessionMap : AuditableEntityMap<PlatformCheckoutSession>
{
    public override void Configure(EntityTypeBuilder<PlatformCheckoutSession> builder)
    {
        base.Configure(builder);

        builder.ToTable("platform_checkout_sessions");

        builder.Property(x => x.LeadId).IsRequired();
        builder.Property(x => x.SubscriptionPlanId).IsRequired();
        builder.Property(x => x.ExternalCheckoutId).HasMaxLength(200);
        builder.Property(x => x.CheckoutUrl).HasMaxLength(2000);
        builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.RawProviderResponse).HasMaxLength(8000);

        builder.HasIndex(x => x.LeadId);
        builder.HasIndex(x => x.SubscriptionPlanId);
        builder.HasIndex(x => x.ExternalCheckoutId).IsUnique();
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}
