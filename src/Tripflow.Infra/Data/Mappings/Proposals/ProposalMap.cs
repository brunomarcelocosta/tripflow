using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Proposals;

namespace Tripflow.Infra.Data.Mappings.Proposals;

public sealed class ProposalMap : AuditableEntityMap<Proposal>
{
    public override void Configure(EntityTypeBuilder<Proposal> builder)
    {
        base.Configure(builder);

        builder.ToTable("proposals");

        builder.Property(x => x.ProposalNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.PublicToken).HasMaxLength(160);
        builder.Property(x => x.PublicUrl).HasMaxLength(1000);
        builder.Property(x => x.PdfUrl).HasMaxLength(1000);
        builder.Property(x => x.ViewedAtUtc);
        builder.Property(x => x.ApprovedAtUtc);
        builder.Property(x => x.RejectedAtUtc);
        builder.Property(x => x.ExpiresAtUtc);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Quote)
            .WithMany(x => x.Proposals)
            .HasForeignKey(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.QuotePricingOption)
            .WithMany()
            .HasForeignKey(x => x.QuotePricingOptionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Versions)
            .WithOne(x => x.Proposal)
            .HasForeignKey(x => x.ProposalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Events)
            .WithOne(x => x.Proposal)
            .HasForeignKey(x => x.ProposalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Payments)
            .WithOne(x => x.Proposal)
            .HasForeignKey(x => x.ProposalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.ProposalNumber })
            .IsUnique();

        builder.HasIndex(x => x.PublicToken)
            .IsUnique();

        builder.HasIndex(x => new { x.TenantId, x.QuoteId });
        builder.HasIndex(x => new { x.TenantId, x.Status });
    }
}
