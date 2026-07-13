using Tripflow.Domain.Entities.Proposals;
using Tripflow.Domain.Entities.Quotes;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Payments;

public sealed class Payment : AuditableEntity, ITenantEntity
{
    private Payment() { }

    public Payment(Guid tenantId, Guid? quoteId, Guid? proposalId, Guid? tenantPaymentProviderId, string? externalPaymentId, PaymentMethod paymentMethod, PaymentStatus status, int? installments, decimal grossAmount, decimal? feeAmount, decimal? netAmount, DateOnly? dueDate, string createdBy)
    {
        TenantId = tenantId;
        QuoteId = quoteId;
        ProposalId = proposalId;
        TenantPaymentProviderId = tenantPaymentProviderId;
        ExternalPaymentId = externalPaymentId;
        PaymentMethod = paymentMethod;
        Status = status;
        Installments = installments;
        GrossAmount = grossAmount;
        FeeAmount = feeAmount;
        NetAmount = netAmount;
        DueDate = dueDate;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid? QuoteId { get; private set; }
    public Quote? Quote { get; private set; }

    public Guid? ProposalId { get; private set; }
    public Proposal? Proposal { get; private set; }

    public Guid? TenantPaymentProviderId { get; private set; }
    public TenantPaymentProvider? TenantPaymentProvider { get; private set; }

    public string? ExternalPaymentId { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public PaymentStatus Status { get; private set; }
    public int? Installments { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal? FeeAmount { get; private set; }
    public decimal? NetAmount { get; private set; }
    public DateOnly? DueDate { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }

    public List<PaymentLink> Links = [];
    public List<FinancialTransaction> FinancialTransactions = [];

    public void SetExternalPaymentId(string? externalPaymentId, string updatedBy)
    {
        ExternalPaymentId = externalPaymentId;
        SetUpdated(updatedBy);
    }

    public void MarkAsPaid(string updatedBy)
    {
        Status = PaymentStatus.Paid;
        PaidAtUtc = DateTime.UtcNow;
        SetUpdated(updatedBy);
    }

    public void Cancel(string updatedBy)
    {
        Status = PaymentStatus.Cancelled;
        SetUpdated(updatedBy);
    }

    public void Refund(string updatedBy)
    {
        Status = PaymentStatus.Refunded;
        SetUpdated(updatedBy);
    }

    public void Fail(string updatedBy)
    {
        Status = PaymentStatus.Failed;
        SetUpdated(updatedBy);
    }

    public void Expire(string updatedBy)
    {
        Status = PaymentStatus.Expired;
        SetUpdated(updatedBy);
    }

    public bool CanBeCancelled()
        => Status is not (PaymentStatus.Paid or PaymentStatus.Refunded or PaymentStatus.Cancelled);

    public bool CanBeRefunded()
        => Status == PaymentStatus.Paid;
}

