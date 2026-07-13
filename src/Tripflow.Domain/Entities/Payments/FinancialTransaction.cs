using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Payments;

public sealed class FinancialTransaction : AuditableEntity, ITenantEntity
{
    private FinancialTransaction() { }

    public FinancialTransaction(Guid tenantId, Guid? paymentId, Guid? quoteId, FinancialTransactionType type, decimal grossAmount, decimal? feeAmount, decimal? netAmount, decimal? agencyCost, decimal? profitAmount, DateTime transactionDateUtc, string? description, string createdBy)
    {
        TenantId = tenantId;
        PaymentId = paymentId;
        QuoteId = quoteId;
        Type = type;
        GrossAmount = grossAmount;
        FeeAmount = feeAmount;
        NetAmount = netAmount;
        AgencyCost = agencyCost;
        ProfitAmount = profitAmount;
        TransactionDateUtc = transactionDateUtc;
        Description = description;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid? PaymentId { get; private set; }
    public Payment? Payment { get; private set; }

    public Guid? QuoteId { get; private set; }
    public Quote? Quote { get; private set; }

    public FinancialTransactionType Type { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal? FeeAmount { get; private set; }
    public decimal? NetAmount { get; private set; }
    public decimal? AgencyCost { get; private set; }
    public decimal? ProfitAmount { get; private set; }
    public DateTime TransactionDateUtc { get; private set; }
    public string? Description { get; private set; }
}

