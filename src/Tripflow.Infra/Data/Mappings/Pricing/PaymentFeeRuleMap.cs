using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Pricing;

namespace Tripflow.Infra.Data.Mappings.Pricing;

public sealed class PaymentFeeRuleMap : AuditableEntityMap<PaymentFeeRule>
{
    public override void Configure(EntityTypeBuilder<PaymentFeeRule> builder)
    {
        base.Configure(builder);

        builder.ToTable("payment_fee_rules");

        builder.Property(x => x.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Installments).IsRequired();
        builder.Property(x => x.FeePercentage).HasPrecision(9, 4).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.PaymentMethod, x.Installments })
            .IsUnique();
    }
}
