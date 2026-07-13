using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Subscriptions;

namespace Tripflow.Infra.Data.Mappings.Subscriptions;

public sealed class LeadMap : AuditableEntityMap<Lead>
{
    public override void Configure(EntityTypeBuilder<Lead> builder)
    {
        base.Configure(builder);

        builder.ToTable("leads");

        builder.Property(x => x.CompanyName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ResponsibleName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Phone).HasMaxLength(50);
        builder.Property(x => x.PlanOfInterest).HasMaxLength(100);
        builder.Property(x => x.Message).HasMaxLength(4000);
        builder.Property(x => x.Source).HasMaxLength(100);
        builder.Property(x => x.SubscriptionPlanId);
        builder.Property(x => x.PlatformCheckoutSessionId);
        builder.Property(x => x.PaymentStatus).IsRequired();
        builder.Property(x => x.PaidAtUtc);
        builder.Property(x => x.ConvertedToTenant).IsRequired();
        builder.Property(x => x.ConvertedTenantId);

        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.ConvertedToTenant);
        builder.HasIndex(x => x.PaymentStatus);
        builder.HasIndex(x => x.SubscriptionPlanId);
    }
}
