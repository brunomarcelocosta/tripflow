using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.CRM;

namespace Tripflow.Infra.Data.Mappings.CRM;

public sealed class TravelerMap : AuditableEntityMap<Traveler>
{
    public override void Configure(EntityTypeBuilder<Traveler> builder)
    {
        base.Configure(builder);

        builder.ToTable("travelers");

        builder.Property(x => x.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.BirthDate);
        builder.Property(x => x.Nationality).HasMaxLength(100);
        builder.Property(x => x.DocumentNumber).HasMaxLength(50);
        builder.Property(x => x.PassportNumber).HasMaxLength(50);
        builder.Property(x => x.PassportExpirationDate);
        builder.Property(x => x.Notes).HasMaxLength(4000);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Travelers)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.CustomerId });
        builder.HasIndex(x => new { x.TenantId, x.FullName });
        builder.HasIndex(x => new { x.TenantId, x.PassportNumber });
        builder.HasIndex(x => new { x.TenantId, x.PassportExpirationDate });
    }
}
