using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.Payments;

namespace Tripflow.Infra.Data.Mappings.Payments;

public sealed class FinancialTransactionMap : AuditableEntityMap<FinancialTransaction>
{
    public override void Configure(EntityTypeBuilder<FinancialTransaction> builder)
    {
        base.Configure(builder);

        builder.ToTable("financial_transactions");

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.GrossAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.FeeAmount).HasPrecision(18, 2);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);
        builder.Property(x => x.AgencyCost).HasPrecision(18, 2);
        builder.Property(x => x.ProfitAmount).HasPrecision(18, 2);
        builder.Property(x => x.TransactionDateUtc).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Payment)
            .WithMany(x => x.FinancialTransactions)
            .HasForeignKey(x => x.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Quote)
            .WithMany()
            .HasForeignKey(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.TransactionDateUtc });
        builder.HasIndex(x => new { x.TenantId, x.Type });
        builder.HasIndex(x => new { x.TenantId, x.PaymentId });
    }
}
