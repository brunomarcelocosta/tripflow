using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Infra.Data.Mappings.Miles;

public sealed class MilesQuoteOptionMap : AuditableEntityMap<MilesQuoteOption>
{
    public override void Configure(EntityTypeBuilder<MilesQuoteOption> builder)
    {
        base.Configure(builder);

        builder.ToTable("miles_quote_options");

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.MilesAmount).IsRequired();
        builder.Property(x => x.BoardingFees).HasPrecision(18, 2);
        builder.Property(x => x.CostPerThousand).HasPrecision(18, 2);
        builder.Property(x => x.EquivalentCost).HasPrecision(18, 2);
        builder.Property(x => x.CashPrice).HasPrecision(18, 2);
        builder.Property(x => x.EstimatedSavings).HasPrecision(18, 2);
        builder.Property(x => x.ServiceFee).HasPrecision(18, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.Selected).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(4000);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Quote)
            .WithMany(x => x.MilesQuoteOptions)
            .HasForeignKey(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.LoyaltyProgram)
            .WithMany()
            .HasForeignKey(x => x.LoyaltyProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.QuoteId });
        builder.HasIndex(x => new { x.TenantId, x.QuoteId, x.Selected });
    }
}
