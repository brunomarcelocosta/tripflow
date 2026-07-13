using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Quotes;

namespace Tripflow.Infra.Data.Mappings.Quotes;

public sealed class QuoteMap : AuditableEntityMap<Quote>
{
    public override void Configure(EntityTypeBuilder<Quote> builder)
    {
        base.Configure(builder);

        builder.ToTable("quotes");

        builder.Property(x => x.QuoteNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Origin).HasMaxLength(100);
        builder.Property(x => x.Destination).HasMaxLength(100);
        builder.Property(x => x.DepartureDate);
        builder.Property(x => x.ReturnDate);
        builder.Property(x => x.Adults).IsRequired();
        builder.Property(x => x.Children).IsRequired();
        builder.Property(x => x.Infants).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(8000);
        builder.Property(x => x.ExpiresAtUtc);

        builder.HasOne(x => x.Tenant)
            .WithMany(x => x.Quotes)
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Quotes)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Itinerary)
            .WithOne(x => x.Quote)
            .HasForeignKey<Itinerary>(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Quote)
            .HasForeignKey(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.FlightItems)
            .WithOne(x => x.Quote)
            .HasForeignKey(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.PricingOptions)
            .WithOne(x => x.Quote)
            .HasForeignKey(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Proposals)
            .WithOne(x => x.Quote)
            .HasForeignKey(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.MilesQuoteOptions)
            .WithOne(x => x.Quote)
            .HasForeignKey(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.QuoteNumber })
            .IsUnique();

        builder.HasIndex(x => new { x.TenantId, x.CustomerId });
        builder.HasIndex(x => new { x.TenantId, x.Status });
        builder.HasIndex(x => new { x.TenantId, x.Type });
        builder.HasIndex(x => new { x.TenantId, x.DepartureDate });
    }
}
