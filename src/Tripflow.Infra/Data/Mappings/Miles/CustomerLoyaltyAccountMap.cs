using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Infra.Data.Mappings.Miles;

public sealed class CustomerLoyaltyAccountMap : AuditableEntityMap<CustomerLoyaltyAccount>
{
    public override void Configure(EntityTypeBuilder<CustomerLoyaltyAccount> builder)
    {
        base.Configure(builder);

        builder.ToTable("customer_loyalty_accounts");

        builder.Property(x => x.AccountNumber).HasMaxLength(100);
        builder.Property(x => x.CurrentBalance).IsRequired();
        builder.Property(x => x.AverageCostPerThousand).HasPrecision(18, 2);
        builder.Property(x => x.LastBalanceUpdateUtc);
        builder.Property(x => x.Notes).HasMaxLength(4000);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.LoyaltyAccounts)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.LoyaltyProgram)
            .WithMany()
            .HasForeignKey(x => x.LoyaltyProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.ExpirationBatches)
            .WithOne(x => x.CustomerLoyaltyAccount)
            .HasForeignKey(x => x.CustomerLoyaltyAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Transactions)
            .WithOne(x => x.CustomerLoyaltyAccount)
            .HasForeignKey(x => x.CustomerLoyaltyAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.CustomerId });
        builder.HasIndex(x => new { x.TenantId, x.LoyaltyProgramId });
        builder.HasIndex(x => new { x.TenantId, x.CustomerId, x.LoyaltyProgramId, x.AccountNumber });
    }
}
