using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Proposals;

namespace Tripflow.Infra.Data.Mappings.Proposals;

public sealed class ProposalVersionMap : AuditableEntityMap<ProposalVersion>
{
    public override void Configure(EntityTypeBuilder<ProposalVersion> builder)
    {
        base.Configure(builder);

        builder.ToTable("proposal_versions");

        builder.Property(x => x.VersionNumber).IsRequired();
        builder.Property(x => x.HtmlSnapshot).HasColumnType("text");
        builder.Property(x => x.PdfUrl).HasMaxLength(1000);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Proposal)
            .WithMany(x => x.Versions)
            .HasForeignKey(x => x.ProposalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.GeneratedByUser)
            .WithMany()
            .HasForeignKey(x => x.GeneratedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.ProposalId, x.VersionNumber })
            .IsUnique();
    }
}
