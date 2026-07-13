using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Quotes;

namespace Tripflow.Infra.Data.Mappings.Quotes;

public sealed class QuoteFlightItemMap : AuditableEntityMap<QuoteFlightItem>
{
    public override void Configure(EntityTypeBuilder<QuoteFlightItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("quote_flight_items");

        builder.Property(x => x.AirlineName).HasMaxLength(100);
        builder.Property(x => x.FareFamily).HasMaxLength(100);
        builder.Property(x => x.BaggageDescription).HasMaxLength(4000);
        builder.Property(x => x.IncludedPersonalItem).IsRequired();
        builder.Property(x => x.IncludedCarryOn).IsRequired();
        builder.Property(x => x.CarryOnWeightKg).HasPrecision(5, 2);
        builder.Property(x => x.IncludedCheckedBag).IsRequired();
        builder.Property(x => x.CheckedBagWeightKg).HasPrecision(5, 2);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Quote)
            .WithMany(x => x.FlightItems)
            .HasForeignKey(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Segments)
            .WithOne(x => x.QuoteFlightItem)
            .HasForeignKey(x => x.QuoteFlightItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.QuoteId });
    }
}
