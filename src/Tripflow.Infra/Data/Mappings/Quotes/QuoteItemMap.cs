using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Quotes;

namespace Tripflow.Infra.Data.Mappings.Quotes;

public sealed class QuoteItemMap : AuditableEntityMap<QuoteItem>
{
    public override void Configure(EntityTypeBuilder<QuoteItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("quote_items");

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description).HasMaxLength(4000);
        builder.Property(x => x.AgencyCost).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.SaleAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(4000);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Quote)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.QuoteId });
        builder.HasIndex(x => new { x.TenantId, x.Type });
    }
}
