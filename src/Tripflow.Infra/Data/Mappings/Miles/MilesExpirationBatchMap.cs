using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Infra.Data.Mappings.Miles;

public sealed class MilesExpirationBatchMap : AuditableEntityMap<MilesExpirationBatch>
{
    public override void Configure(EntityTypeBuilder<MilesExpirationBatch> builder)
    {
        base.Configure(builder);

        builder.ToTable("miles_expiration_batches");

        builder.Property(x => x.Amount).IsRequired();
        builder.Property(x => x.RemainingAmount).IsRequired();
        builder.Property(x => x.ExpiresAt).IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CustomerLoyaltyAccount)
            .WithMany(x => x.ExpirationBatches)
            .HasForeignKey(x => x.CustomerLoyaltyAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.CustomerLoyaltyAccountId });
        builder.HasIndex(x => new { x.TenantId, x.ExpiresAt });
        builder.HasIndex(x => new { x.TenantId, x.Status });
    }
}
