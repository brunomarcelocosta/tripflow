using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Tenants;

namespace Tripflow.Infra.Data.Mappings;

public sealed class TenantCommercialSettingsMap : AuditableEntityMap<TenantCommercialSettings>
{
    public override void Configure(EntityTypeBuilder<TenantCommercialSettings> builder)
    {
        base.Configure(builder);

        builder.ToTable("tenant_commercial_settings");

        builder.Property(x => x.CommercialEmail).HasMaxLength(200);
        builder.Property(x => x.CommercialPhone).HasMaxLength(50);
        builder.Property(x => x.WhatsApp).HasMaxLength(50);
        builder.Property(x => x.Instagram).HasMaxLength(150);
        builder.Property(x => x.Website).HasMaxLength(300);

        builder.Property(x => x.DefaultProfitAmount).HasPrecision(18, 2);
        builder.Property(x => x.DefaultProfitPercentage).HasPrecision(9, 4);
        builder.Property(x => x.DefaultPixDiscountPercentage).HasPrecision(9, 4);

        builder.Property(x => x.DefaultProposalExpirationHours).IsRequired();
        builder.Property(x => x.DefaultTerms).HasMaxLength(8000);
        builder.Property(x => x.DefaultImportantNotes).HasMaxLength(8000);

        builder.HasOne(x => x.Tenant)
            .WithOne(x => x.CommercialSettings)
            .HasForeignKey<TenantCommercialSettings>(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TenantId)
            .IsUnique();
    }
}
