using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Subscriptions;

namespace Tripflow.Infra.Data.Mappings.Subscriptions;

public sealed class TenantUsageMap : BaseEntityMap<TenantUsage>
{
    public override void Configure(EntityTypeBuilder<TenantUsage> builder)
    {
        base.Configure(builder);

        builder.ToTable("tenant_usages");

        builder.Property(x => x.UsageType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.PeriodYear).IsRequired();
        builder.Property(x => x.PeriodMonth).IsRequired();
        builder.Property(x => x.CurrentValue).IsRequired();
        builder.Property(x => x.LimitValue);
        builder.Property(x => x.UpdatedAtUtc).IsRequired();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.UsageType, x.PeriodYear, x.PeriodMonth })
            .IsUnique();
    }
}
