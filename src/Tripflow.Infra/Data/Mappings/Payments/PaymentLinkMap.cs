using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Payments;

namespace Tripflow.Infra.Data.Mappings.Payments;

public sealed class PaymentLinkMap : AuditableEntityMap<PaymentLink>
{
    public override void Configure(EntityTypeBuilder<PaymentLink> builder)
    {
        base.Configure(builder);

        builder.ToTable("payment_links");

        builder.Property(x => x.ExternalLinkId).HasMaxLength(150);

        builder.Property(x => x.Url)
            .HasMaxLength(1500)
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Payment)
            .WithMany(x => x.Links)
            .HasForeignKey(x => x.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.PaymentId });
        builder.HasIndex(x => new { x.TenantId, x.Status });
        builder.HasIndex(x => x.ExternalLinkId);
    }
}
