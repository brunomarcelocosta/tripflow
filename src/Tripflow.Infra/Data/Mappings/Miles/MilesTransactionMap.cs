using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Infra.Data.Mappings.Miles;

public sealed class MilesTransactionMap : AuditableEntityMap<MilesTransaction>
{
    public override void Configure(EntityTypeBuilder<MilesTransaction> builder)
    {
        base.Configure(builder);

        builder.ToTable("miles_transactions");

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Amount).IsRequired();
        builder.Property(x => x.CostPerThousand).HasPrecision(18, 2);
        builder.Property(x => x.TotalCost).HasPrecision(18, 2);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.TransactionDateUtc).IsRequired();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CustomerLoyaltyAccount)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.CustomerLoyaltyAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.CustomerLoyaltyAccountId });
        builder.HasIndex(x => new { x.TenantId, x.TransactionDateUtc });
        builder.HasIndex(x => new { x.TenantId, x.Type });
    }
}
