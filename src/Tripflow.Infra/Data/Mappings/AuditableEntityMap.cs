using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities;

namespace Tripflow.Infra.Data.Mappings;

public abstract class AuditableEntityMap<T> : BaseEntityMap<T>
    where T : AuditableEntity
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(120).IsRequired();

        builder.Property(x => x.UpdatedAtUtc);
        builder.Property(x => x.UpdatedBy).HasMaxLength(120);

        builder.Property(x => x.IsDeleted).IsRequired();

        builder.Property(x => x.DeletedAtUtc);
        builder.Property(x => x.DeletedBy).HasMaxLength(120);
    }
}
