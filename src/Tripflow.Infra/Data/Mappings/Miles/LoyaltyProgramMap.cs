using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Miles;

namespace Tripflow.Infra.Data.Mappings.Miles;

public sealed class LoyaltyProgramMap : AuditableEntityMap<LoyaltyProgram>
{
    public override void Configure(EntityTypeBuilder<LoyaltyProgram> builder)
    {
        base.Configure(builder);

        builder.ToTable("loyalty_programs");

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Country).HasMaxLength(100);
        builder.Property(x => x.AirlineName).HasMaxLength(100);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.Status);
    }
}
