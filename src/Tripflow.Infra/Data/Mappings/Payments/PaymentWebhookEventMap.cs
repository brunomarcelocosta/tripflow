using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Payments;

namespace Tripflow.Infra.Data.Mappings.Payments;

public sealed class PaymentWebhookEventMap : BaseEntityMap<PaymentWebhookEvent>
{
    public override void Configure(EntityTypeBuilder<PaymentWebhookEvent> builder)
    {
        base.Configure(builder);

        builder.ToTable("payment_webhook_events");

        builder.Property(x => x.ProviderCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ExternalEventId)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.ExternalPaymentId)
            .HasMaxLength(150);

        builder.Property(x => x.Payload)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.Processed).IsRequired();
        builder.Property(x => x.ProcessedAtUtc);
        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasIndex(x => new { x.ProviderCode, x.ExternalEventId })
            .IsUnique();

        builder.HasIndex(x => new { x.ProviderCode, x.ExternalPaymentId });
        builder.HasIndex(x => x.Processed);
    }
}
