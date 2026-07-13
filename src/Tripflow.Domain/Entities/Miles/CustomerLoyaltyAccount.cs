using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Miles;

public sealed class CustomerLoyaltyAccount : AuditableEntity, ITenantEntity
{
    private CustomerLoyaltyAccount() { }

    public CustomerLoyaltyAccount(Guid tenantId, Guid customerId, Guid loyaltyProgramId, string? accountNumber, int currentBalance, decimal? averageCostPerThousand, string? notes, LoyaltyAccountStatus status, string createdBy)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        LoyaltyProgramId = loyaltyProgramId;
        AccountNumber = accountNumber;
        CurrentBalance = currentBalance;
        AverageCostPerThousand = averageCostPerThousand;
        LastBalanceUpdateUtc = DateTime.UtcNow;
        Notes = notes;
        Status = status;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = default!;

    public Guid LoyaltyProgramId { get; private set; }
    public LoyaltyProgram LoyaltyProgram { get; private set; } = default!;

    public string? AccountNumber { get; private set; }
    public int CurrentBalance { get; private set; }
    public decimal? AverageCostPerThousand { get; private set; }
    public DateTime? LastBalanceUpdateUtc { get; private set; }
    public string? Notes { get; private set; }
    public LoyaltyAccountStatus Status { get; private set; }

    public List<MilesExpirationBatch> ExpirationBatches = [];
    public List<MilesTransaction> Transactions = [];

    public void Update(Guid loyaltyProgramId, string? accountNumber, int currentBalance, decimal? averageCostPerThousand, string? notes, LoyaltyAccountStatus status, string updatedBy)
    {
        LoyaltyProgramId = loyaltyProgramId;
        AccountNumber = accountNumber;
        if (currentBalance != CurrentBalance)
        {
            CurrentBalance = currentBalance;
            LastBalanceUpdateUtc = DateTime.UtcNow;
        }
        AverageCostPerThousand = averageCostPerThousand;
        Notes = notes;
        Status = status;
        SetUpdated(updatedBy);
    }

    public void Activate(string updatedBy)
    {
        Status = LoyaltyAccountStatus.Active;
        SetUpdated(updatedBy);
    }

    public void Inactivate(string updatedBy)
    {
        Status = LoyaltyAccountStatus.Inactive;
        SetUpdated(updatedBy);
    }

    public void Suspend(string updatedBy)
    {
        Status = LoyaltyAccountStatus.Suspended;
        SetUpdated(updatedBy);
    }

    public bool CanTransact() => Status == LoyaltyAccountStatus.Active;

    public void AddMiles(int amount, string updatedBy)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Quantidade de milhas deve ser maior que zero.");

        CurrentBalance += amount;
        LastBalanceUpdateUtc = DateTime.UtcNow;
        SetUpdated(updatedBy);
    }

    public void DebitMiles(int amount, string updatedBy)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Quantidade de milhas deve ser maior que zero.");

        if (CurrentBalance - amount < 0)
            throw new InvalidOperationException("Saldo insuficiente para débito de milhas.");

        CurrentBalance -= amount;
        LastBalanceUpdateUtc = DateTime.UtcNow;
        SetUpdated(updatedBy);
    }

    public void AdjustBalance(int amount, string updatedBy)
    {
        if (amount < 0 && CurrentBalance + amount < 0)
            throw new InvalidOperationException("Ajuste resultaria em saldo negativo.");

        CurrentBalance += amount;
        LastBalanceUpdateUtc = DateTime.UtcNow;
        SetUpdated(updatedBy);
    }

    public void UpdateAverageCost(decimal? averageCostPerThousand, string updatedBy)
    {
        if (averageCostPerThousand is < 0)
            throw new InvalidOperationException("Custo médio por mil não pode ser negativo.");

        AverageCostPerThousand = averageCostPerThousand;
        SetUpdated(updatedBy);
    }
}
