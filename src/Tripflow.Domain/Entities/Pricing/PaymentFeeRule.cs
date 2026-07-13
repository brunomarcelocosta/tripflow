using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Pricing;

public sealed class PaymentFeeRule : AuditableEntity, ITenantEntity
{
    private PaymentFeeRule() { }

    public PaymentFeeRule(Guid tenantId, PaymentMethod paymentMethod, int installments, decimal feePercentage, bool isActive, string createdBy)
    {
        TenantId = tenantId;
        PaymentMethod = paymentMethod;
        Installments = installments;
        FeePercentage = feePercentage;
        IsActive = isActive;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public PaymentMethod PaymentMethod { get; private set; }
    public int Installments { get; private set; }
    public decimal FeePercentage { get; private set; }
    public bool IsActive { get; private set; }

    public void Update(decimal feePercentage, bool isActive, string updatedBy)
    {
        FeePercentage = feePercentage;
        IsActive = isActive;
        SetUpdated(updatedBy);
    }
}

