using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Audit;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Infra.Data.Mappings;

namespace Tripflow.Infra.Data.Mappings.Audit;

public class AuditLogMap : IEntityTypeConfiguration<AuditLog>
{
    public virtual void Configure(EntityTypeBuilder<AuditLog> builder)
    {

        builder.ToTable("audit_log");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId);

        builder.Property(x => x.UserId);

        builder.Property(x => x.Action);

        builder.Property(x => x.EntityName);

        builder.Property(x => x.EntityId);

        builder.Property(x => x.OldValuesJson);

        builder.Property(x => x.NewValuesJson);

        builder.Property(x => x.IpAddress);

        builder.Property(x => x.UserAgent);

        builder.Property(x => x.CorrelationId);

        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}