using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Miles;

public sealed class MilesTransaction : AuditableEntity, ITenantEntity
{
    private MilesTransaction() { }

    private MilesTransaction(Guid tenantId, Guid customerLoyaltyAccountId, MilesTransactionType type, int amount, decimal? costPerThousand, decimal? totalCost, string? description, DateTime transactionDateUtc, string createdBy)
    {
        TenantId = tenantId;
        CustomerLoyaltyAccountId = customerLoyaltyAccountId;
        Type = type;
        Amount = amount;
        CostPerThousand = costPerThousand;
        TotalCost = totalCost;
        Description = description;
        TransactionDateUtc = transactionDateUtc;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid CustomerLoyaltyAccountId { get; private set; }
    public CustomerLoyaltyAccount CustomerLoyaltyAccount { get; private set; } = default!;

    public MilesTransactionType Type { get; private set; }
    public int Amount { get; private set; }
    public decimal? CostPerThousand { get; private set; }
    public decimal? TotalCost { get; private set; }
    public string? Description { get; private set; }
    public DateTime TransactionDateUtc { get; private set; }

    public static MilesTransaction CreateCredit(
        Guid tenantId,
        Guid customerLoyaltyAccountId,
        int amount,
        decimal? costPerThousand,
        string? description,
        DateTime transactionDateUtc,
        string createdBy)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Quantidade deve ser maior que zero para crédito.");

        var totalCost = CalculateTotalCost(amount, costPerThousand);
        return new MilesTransaction(tenantId, customerLoyaltyAccountId, MilesTransactionType.Credit, amount, costPerThousand, totalCost, description, transactionDateUtc, createdBy);
    }

    public static MilesTransaction CreateDebit(
        Guid tenantId,
        Guid customerLoyaltyAccountId,
        int amount,
        decimal? costPerThousand,
        string? description,
        DateTime transactionDateUtc,
        string createdBy)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Quantidade deve ser maior que zero para débito.");

        var totalCost = CalculateTotalCost(amount, costPerThousand);
        return new MilesTransaction(tenantId, customerLoyaltyAccountId, MilesTransactionType.Debit, amount, costPerThousand, totalCost, description, transactionDateUtc, createdBy);
    }

    public static MilesTransaction CreateAdjustment(
        Guid tenantId,
        Guid customerLoyaltyAccountId,
        int amount,
        string? description,
        DateTime transactionDateUtc,
        string createdBy)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Quantidade deve ser maior que zero para ajuste positivo.");

        return new MilesTransaction(tenantId, customerLoyaltyAccountId, MilesTransactionType.Adjustment, amount, null, null, description, transactionDateUtc, createdBy);
    }

    public static MilesTransaction CreateExpiration(
        Guid tenantId,
        Guid customerLoyaltyAccountId,
        int amount,
        string? description,
        DateTime transactionDateUtc,
        string createdBy)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Quantidade deve ser maior que zero para expiração.");

        return new MilesTransaction(tenantId, customerLoyaltyAccountId, MilesTransactionType.Expiration, amount, null, null, description, transactionDateUtc, createdBy);
    }

    public static decimal? CalculateTotalCost(int amount, decimal? costPerThousand)
    {
        if (costPerThousand is null)
            return null;

        return amount / 1000m * costPerThousand.Value;
    }
}
