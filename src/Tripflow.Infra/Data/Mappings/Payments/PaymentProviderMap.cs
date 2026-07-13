using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Payments;

namespace Tripflow.Infra.Data.Mappings.Payments;

public sealed class PaymentProviderMap : BaseEntityMap<PaymentProvider>
{
    public override void Configure(EntityTypeBuilder<PaymentProvider> builder)
    {
        base.Configure(builder);

        builder.ToTable("payment_providers");

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();
    }
}
