using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Pricing;

namespace Tripflow.Infra.Data.Mappings.Pricing;

public sealed class QuotePaymentConditionMap : AuditableEntityMap<QuotePaymentCondition>
{
    public override void Configure(EntityTypeBuilder<QuotePaymentCondition> builder)
    {
        base.Configure(builder);

        builder.ToTable("quote_payment_conditions");

        builder.Property(x => x.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Installments).IsRequired();
        builder.Property(x => x.FeePercentage).HasPrecision(9, 4);
        builder.Property(x => x.GrossAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.InstallmentAmount).HasPrecision(18, 2);
        builder.Property(x => x.EstimatedFeeAmount).HasPrecision(18, 2);
        builder.Property(x => x.EstimatedNetAmount).HasPrecision(18, 2);
        builder.Property(x => x.EstimatedProfitAmount).HasPrecision(18, 2);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.QuotePricingOption)
            .WithMany(x => x.PaymentConditions)
            .HasForeignKey(x => x.QuotePricingOptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.QuotePricingOptionId, x.PaymentMethod, x.Installments })
            .IsUnique();
    }
}
