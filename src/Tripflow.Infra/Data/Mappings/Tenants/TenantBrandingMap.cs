using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Tenants;

namespace Tripflow.Infra.Data.Mappings;

public sealed class TenantBrandingMap : AuditableEntityMap<TenantBranding>
{
    public override void Configure(EntityTypeBuilder<TenantBranding> builder)
    {
        base.Configure(builder);

        builder.ToTable("tenant_brandings");

        builder.Property(x => x.LogoUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.PrimaryColor)
            .HasMaxLength(20);

        builder.Property(x => x.SecondaryColor)
            .HasMaxLength(20);

        builder.Property(x => x.TextColor)
            .HasMaxLength(20);

        builder.Property(x => x.ProposalFooter)
            .HasMaxLength(4000);

        builder.HasOne(x => x.Tenant)
            .WithOne(x => x.Branding)
            .HasForeignKey<TenantBranding>(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TenantId)
            .IsUnique();
    }
}
