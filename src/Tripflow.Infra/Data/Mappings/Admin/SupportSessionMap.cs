using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Admin;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Tenants;

namespace Tripflow.Infra.Data.Mappings.Admin;

public sealed class SupportSessionMap : IEntityTypeConfiguration<SupportSession>
{
    public void Configure(EntityTypeBuilder<SupportSession> builder)
    {
        builder.ToTable("support_sessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.AdminIdentityProviderUserId).IsRequired();
        builder.Property(x => x.Reason).IsRequired();
        builder.Property(x => x.StartedAtUtc).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedBy).IsRequired();

        builder.HasOne(x => x.AdminUserProfile)
            .WithMany()
            .HasForeignKey(x => x.AdminUserProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TargetTenant)
            .WithMany()
            .HasForeignKey(x => x.TargetTenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.AdminUserProfileId, x.IsActive });
        builder.HasIndex(x => x.TargetTenantId);
        builder.HasIndex(x => x.StartedAtUtc);
    }
}
