using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Miles;

public sealed class MilesExpirationBatch : AuditableEntity, ITenantEntity
{
    private MilesExpirationBatch() { }

    public MilesExpirationBatch(Guid tenantId, Guid customerLoyaltyAccountId, int amount, DateOnly expiresAt, MilesExpirationStatus status, string createdBy)
    {
        TenantId = tenantId;
        CustomerLoyaltyAccountId = customerLoyaltyAccountId;
        Amount = amount;
        RemainingAmount = amount;
        ExpiresAt = expiresAt;
        Status = status;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid CustomerLoyaltyAccountId { get; private set; }
    public CustomerLoyaltyAccount CustomerLoyaltyAccount { get; private set; } = default!;

    public int Amount { get; private set; }
    public int RemainingAmount { get; private set; }
    public DateOnly ExpiresAt { get; private set; }
    public MilesExpirationStatus Status { get; private set; }

    public void Update(int amount, DateOnly expiresAt, string updatedBy)
    {
        if (Status != MilesExpirationStatus.Pending)
            throw new InvalidOperationException("Somente lotes pendentes podem ser alterados.");

        Amount = amount;
        RemainingAmount = amount;
        ExpiresAt = expiresAt;
        SetUpdated(updatedBy);
    }

    public void Cancel(string updatedBy)
    {
        if (Status != MilesExpirationStatus.Pending)
            throw new InvalidOperationException("Somente lotes pendentes podem ser cancelados.");

        Status = MilesExpirationStatus.Cancelled;
        SetUpdated(updatedBy);
    }

    public void MarkAsExpired(string updatedBy)
    {
        if (Status != MilesExpirationStatus.Pending)
            throw new InvalidOperationException("Somente lotes pendentes podem expirar.");

        Status = MilesExpirationStatus.Expired;
        RemainingAmount = 0;
        SetUpdated(updatedBy);
    }

    public int Consume(int amount, string updatedBy)
    {
        if (Status != MilesExpirationStatus.Pending)
            return 0;

        if (amount <= 0)
            return 0;

        var consumed = Math.Min(amount, RemainingAmount);
        RemainingAmount -= consumed;

        if (RemainingAmount == 0)
            Status = MilesExpirationStatus.Used;

        SetUpdated(updatedBy);
        return consumed;
    }
}
