using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Platform;

namespace Tripflow.Infra.Data.Mappings.Platform;

public sealed class PlatformPaymentEventMap : IEntityTypeConfiguration<PlatformPaymentEvent>
{
    public void Configure(EntityTypeBuilder<PlatformPaymentEvent> builder)
    {
        builder.ToTable("platform_payment_events");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProviderCode).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ExternalEventId).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ExternalCheckoutId).HasMaxLength(200);
        builder.Property(x => x.Payload).IsRequired();
        builder.Property(x => x.Processed).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasIndex(x => new { x.ProviderCode, x.ExternalEventId }).IsUnique();
        builder.HasIndex(x => x.ExternalCheckoutId);
        builder.HasIndex(x => x.Processed);
    }
}
