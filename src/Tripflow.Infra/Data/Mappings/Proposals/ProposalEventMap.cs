using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Proposals;

namespace Tripflow.Infra.Data.Mappings.Proposals;

public sealed class ProposalEventMap : BaseEntityMap<ProposalEvent>
{
    public override void Configure(EntityTypeBuilder<ProposalEvent> builder)
    {
        base.Configure(builder);

        builder.ToTable("proposal_events");

        builder.Property(x => x.EventType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.IpAddress).HasMaxLength(100);
        builder.Property(x => x.UserAgent).HasMaxLength(1000);
        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Proposal)
            .WithMany(x => x.Events)
            .HasForeignKey(x => x.ProposalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.ProposalId, x.CreatedAtUtc });
        builder.HasIndex(x => new { x.TenantId, x.EventType });
    }
}
