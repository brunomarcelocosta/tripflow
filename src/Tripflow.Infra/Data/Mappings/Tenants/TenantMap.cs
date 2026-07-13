using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Entities.Tenants;

namespace Tripflow.Infra.Data.Mappings;

// SaaS / Tenant


public sealed class TenantMap : AuditableEntityMap<Tenant>
{
    public override void Configure(EntityTypeBuilder<Tenant> builder)
    {
        base.Configure(builder);

        builder.ToTable("tenants");

        builder.Property(x => x.LegalName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.TradeName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.DocumentNumber)
            .HasMaxLength(30);

        builder.Property(x => x.Email)
            .HasMaxLength(200);

        builder.Property(x => x.Phone)
            .HasMaxLength(50);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(x => x.Branding)
            .WithOne(x => x.Tenant)
            .HasForeignKey<TenantBranding>(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CommercialSettings)
            .WithOne(x => x.Tenant)
            .HasForeignKey<TenantCommercialSettings>(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CurrentSubscription)
            .WithOne(x => x.Tenant)
            .HasForeignKey<TenantSubscription>(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.DocumentNumber);
        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.Status);
    }
}
