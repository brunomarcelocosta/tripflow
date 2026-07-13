using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Quotes;

namespace Tripflow.Infra.Data.Mappings.Quotes;

public sealed class FlightSegmentMap : AuditableEntityMap<FlightSegment>
{
    public override void Configure(EntityTypeBuilder<FlightSegment> builder)
    {
        base.Configure(builder);

        builder.ToTable("flight_segments");

        builder.Property(x => x.OriginAirport)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.DestinationAirport)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.OriginCity).HasMaxLength(100);
        builder.Property(x => x.DestinationCity).HasMaxLength(100);
        builder.Property(x => x.DepartureDateTimeUtc);
        builder.Property(x => x.ArrivalDateTimeUtc);
        builder.Property(x => x.FlightNumber).HasMaxLength(50);
        builder.Property(x => x.AirlineName).HasMaxLength(100);
        builder.Property(x => x.Sequence).IsRequired();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.QuoteFlightItem)
            .WithMany(x => x.Segments)
            .HasForeignKey(x => x.QuoteFlightItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.QuoteFlightItemId, x.Sequence })
            .IsUnique();

        builder.HasIndex(x => new { x.TenantId, x.DepartureDateTimeUtc });
    }
}
