using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Quotes;

namespace Tripflow.Infra.Data.Mappings.Quotes;

public sealed class ItineraryMap : AuditableEntityMap<Itinerary>
{
    public override void Configure(EntityTypeBuilder<Itinerary> builder)
    {
        base.Configure(builder);

        builder.ToTable("itineraries");

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.TotalDays);
        builder.Property(x => x.TotalNights);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Quote)
            .WithOne(x => x.Itinerary)
            .HasForeignKey<Itinerary>(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Stops)
            .WithOne(x => x.Itinerary)
            .HasForeignKey(x => x.ItineraryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.QuoteId })
            .IsUnique();
    }
}
