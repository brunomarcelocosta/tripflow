using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities;

namespace Tripflow.Infra.Data.Mappings.Quotes;

public sealed class ItineraryStopMap : AuditableEntityMap<ItineraryStop>
{
    public override void Configure(EntityTypeBuilder<ItineraryStop> builder)
    {
        base.Configure(builder);

        builder.ToTable("itinerary_stops");

        builder.Property(x => x.Country)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Nights).IsRequired();
        builder.Property(x => x.Sequence).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(4000);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Itinerary)
            .WithMany(x => x.Stops)
            .HasForeignKey(x => x.ItineraryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.ItineraryId, x.Sequence })
            .IsUnique();
    }
}
