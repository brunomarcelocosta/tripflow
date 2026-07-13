using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Identity;

namespace Tripflow.Infra.Data.Mappings.Identity;

public sealed class UserInvitationMap : AuditableEntityMap<UserInvitation>
{
    public override void Configure(EntityTypeBuilder<UserInvitation> builder)
    {
        base.Configure(builder);

        builder.ToTable("user_invitations");

        builder.Property(x => x.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.TokenHash)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc).IsRequired();
        builder.Property(x => x.AcceptedAtUtc);
        builder.Property(x => x.CancelledAtUtc);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.Email });
        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.HasIndex(x => x.ExpiresAtUtc);
    }
}
