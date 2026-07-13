using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Pricing;

namespace Tripflow.Infra.Data.Mappings.Pricing;

public sealed class QuotePricingOptionMap : AuditableEntityMap<QuotePricingOption>
{
    public override void Configure(EntityTypeBuilder<QuotePricingOption> builder)
    {
        base.Configure(builder);

        builder.ToTable("quote_pricing_options");

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.AgencyCost).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.DesiredProfitAmount).HasPrecision(18, 2);
        builder.Property(x => x.DesiredProfitPercentage).HasPrecision(9, 4);
        builder.Property(x => x.PixDiscountPercentage).HasPrecision(9, 4);
        builder.Property(x => x.PixAmount).HasPrecision(18, 2);
        builder.Property(x => x.CreditCashAmount).HasPrecision(18, 2);
        builder.Property(x => x.Selected).IsRequired();
        builder.Property(x => x.InternalNotes).HasMaxLength(4000);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Quote)
            .WithMany(x => x.PricingOptions)
            .HasForeignKey(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.PaymentConditions)
            .WithOne(x => x.QuotePricingOption)
            .HasForeignKey(x => x.QuotePricingOptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.QuoteId });
        builder.HasIndex(x => new { x.TenantId, x.QuoteId, x.Selected });
    }
}
