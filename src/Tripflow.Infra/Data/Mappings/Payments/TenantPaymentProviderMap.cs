using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Payments;

namespace Tripflow.Infra.Data.Mappings.Payments;

public sealed class TenantPaymentProviderMap : AuditableEntityMap<TenantPaymentProvider>
{
    public override void Configure(EntityTypeBuilder<TenantPaymentProvider> builder)
    {
        base.Configure(builder);

        builder.ToTable("tenant_payment_providers");

        builder.Property(x => x.DisplayName).HasMaxLength(100);
        builder.Property(x => x.EncryptedApiKey).HasMaxLength(4000);
        builder.Property(x => x.EncryptedSecret).HasMaxLength(4000);
        builder.Property(x => x.WebhookSecret).HasMaxLength(1000);
        builder.Property(x => x.IsDefault).IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PaymentProvider)
            .WithMany()
            .HasForeignKey(x => x.PaymentProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.PaymentProviderId });
        builder.HasIndex(x => new { x.TenantId, x.IsDefault });
    }
}
