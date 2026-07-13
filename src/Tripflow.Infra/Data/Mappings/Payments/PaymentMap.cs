using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Payments;

namespace Tripflow.Infra.Data.Mappings.Payments;

public sealed class PaymentMap : AuditableEntityMap<Payment>
{
    public override void Configure(EntityTypeBuilder<Payment> builder)
    {
        base.Configure(builder);

        builder.ToTable("payments");

        builder.Property(x => x.ExternalPaymentId).HasMaxLength(150);

        builder.Property(x => x.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Installments);
        builder.Property(x => x.GrossAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.FeeAmount).HasPrecision(18, 2);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);
        builder.Property(x => x.DueDate);
        builder.Property(x => x.PaidAtUtc);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Quote)
            .WithMany()
            .HasForeignKey(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Proposal)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.ProposalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TenantPaymentProvider)
            .WithMany()
            .HasForeignKey(x => x.TenantPaymentProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Links)
            .WithOne(x => x.Payment)
            .HasForeignKey(x => x.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.FinancialTransactions)
            .WithOne(x => x.Payment)
            .HasForeignKey(x => x.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.Status });
        builder.HasIndex(x => new { x.TenantId, x.ProposalId });
        builder.HasIndex(x => new { x.TenantId, x.ExternalPaymentId });
    }
}
