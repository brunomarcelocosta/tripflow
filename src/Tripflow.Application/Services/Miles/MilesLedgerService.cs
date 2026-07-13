using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.Services.Miles;

public interface IMilesLedgerService
{
    Task<MilesTransaction> ProcessTransactionAsync(
        Guid tenantId,
        CreateMilesTransactionRequest request,
        string createdBy,
        CancellationToken cancellationToken = default);
}

public sealed class MilesLedgerService(
    IMilesTransactionRepository milesTransactionRepository,
    ICustomerLoyaltyAccountRepository customerLoyaltyAccountRepository,
    IMilesExpirationBatchRepository milesExpirationBatchRepository) : IMilesLedgerService
{
    public async Task<MilesTransaction> ProcessTransactionAsync(
        Guid tenantId,
        CreateMilesTransactionRequest request,
        string createdBy,
        CancellationToken cancellationToken = default)
    {
        await milesTransactionRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var account = await customerLoyaltyAccountRepository.GetByIdAsync(request.CustomerLoyaltyAccountId);
            if (account is null || account.TenantId != tenantId)
                throw new InvalidOperationException("Conta de fidelidade não encontrada.");

            if (!account.CanTransact())
                throw new InvalidOperationException("A conta de fidelidade não está ativa para transações.");

            var transactionDateUtc = request.TransactionDateUtc ?? DateTime.UtcNow;
            MilesTransaction transaction;

            switch (request.Type)
            {
                case MilesTransactionType.Credit:
                    transaction = MilesTransaction.CreateCredit(
                        tenantId,
                        account.Id,
                        request.Amount,
                        request.CostPerThousand,
                        request.Description,
                        transactionDateUtc,
                        createdBy);

                    account.AddMiles(request.Amount, createdBy);

                    if (request.CreateExpirationBatch && request.ExpiresAt.HasValue)
                    {
                        var batch = new MilesExpirationBatch(
                            tenantId,
                            account.Id,
                            request.Amount,
                            request.ExpiresAt.Value,
                            MilesExpirationStatus.Pending,
                            createdBy);

                        await milesExpirationBatchRepository.AddAsync(batch, cancellationToken);
                    }
                    break;

                case MilesTransactionType.Debit:
                    transaction = MilesTransaction.CreateDebit(
                        tenantId,
                        account.Id,
                        request.Amount,
                        request.CostPerThousand,
                        request.Description,
                        transactionDateUtc,
                        createdBy);

                    account.DebitMiles(request.Amount, createdBy);
                    await ConsumePendingBatchesAsync(account.Id, tenantId, request.Amount, createdBy, cancellationToken);
                    break;

                case MilesTransactionType.Adjustment:
                    transaction = MilesTransaction.CreateAdjustment(
                        tenantId,
                        account.Id,
                        request.Amount,
                        request.Description,
                        transactionDateUtc,
                        createdBy);

                    account.AdjustBalance(request.Amount, createdBy);
                    break;

                case MilesTransactionType.Expiration:
                    transaction = MilesTransaction.CreateExpiration(
                        tenantId,
                        account.Id,
                        request.Amount,
                        request.Description,
                        transactionDateUtc,
                        createdBy);

                    account.DebitMiles(request.Amount, createdBy);
                    await ConsumePendingBatchesAsync(account.Id, tenantId, request.Amount, createdBy, cancellationToken);
                    break;

                default:
                    throw new InvalidOperationException("Tipo de transação de milhas inválido.");
            }

            await milesTransactionRepository.AddAsync(transaction, cancellationToken);
            await customerLoyaltyAccountRepository.UpdateAsync(account, cancellationToken);

            await milesTransactionRepository.CommitTransactionAsync(cancellationToken);
            return transaction;
        }
        catch
        {
            await milesTransactionRepository.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task ConsumePendingBatchesAsync(
        Guid accountId,
        Guid tenantId,
        int amountToConsume,
        string updatedBy,
        CancellationToken cancellationToken)
    {
        if (amountToConsume <= 0)
            return;

        var remainingToConsume = amountToConsume;
        var pendingBatches = await milesExpirationBatchRepository.GetPendingByAccountOrderedAsync(accountId, tenantId, cancellationToken);

        foreach (var batch in pendingBatches)
        {
            if (remainingToConsume <= 0)
                break;

            var consumed = batch.Consume(remainingToConsume, updatedBy);
            if (consumed > 0)
            {
                await milesExpirationBatchRepository.UpdateAsync(batch, cancellationToken);
                remainingToConsume -= consumed;
            }
        }
    }
}
