using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.Services.Payments;

public interface IFinancialTransactionService
{
    Task RegisterSaleAsync(Payment payment, decimal? agencyCost, string createdBy, CancellationToken cancellationToken = default);
    Task RegisterRefundAsync(Payment payment, decimal? agencyCost, string createdBy, CancellationToken cancellationToken = default);
    Task RegisterChargebackAsync(Payment payment, decimal? agencyCost, string createdBy, CancellationToken cancellationToken = default);
}

public class FinancialTransactionService(
    IFinancialTransactionRepository transactionRepository) : IFinancialTransactionService
{
    public async Task RegisterSaleAsync(Payment payment, decimal? agencyCost, string createdBy, CancellationToken cancellationToken = default)
    {
        if (await transactionRepository.ExistsSaleByPaymentIdAsync(payment.Id, payment.TenantId, cancellationToken))
            return;

        var net = payment.NetAmount ?? payment.GrossAmount;
        var fee = payment.FeeAmount;
        var profit = agencyCost.HasValue ? net - agencyCost.Value : (decimal?)null;

        var tx = new FinancialTransaction(
            payment.TenantId,
            payment.Id,
            payment.QuoteId,
            Domain.Enums.FinancialTransactionType.Sale,
            payment.GrossAmount,
            fee,
            net,
            agencyCost,
            profit,
            DateTime.UtcNow,
            "Venda registrada",
            createdBy);

        await transactionRepository.AddAsync(tx, cancellationToken);
    }

    public async Task RegisterRefundAsync(Payment payment, decimal? agencyCost, string createdBy, CancellationToken cancellationToken = default)
    {
        var net = payment.NetAmount ?? payment.GrossAmount;
        var profit = agencyCost.HasValue ? -(net - agencyCost.Value) : (decimal?)null;

        var tx = new FinancialTransaction(
            payment.TenantId,
            payment.Id,
            payment.QuoteId,
            Domain.Enums.FinancialTransactionType.Refund,
            payment.GrossAmount,
            payment.FeeAmount,
            net,
            agencyCost,
            profit,
            DateTime.UtcNow,
            "Estorno registrado",
            createdBy);

        await transactionRepository.AddAsync(tx, cancellationToken);
    }

    public async Task RegisterChargebackAsync(Payment payment, decimal? agencyCost, string createdBy, CancellationToken cancellationToken = default)
    {
        var net = payment.NetAmount ?? payment.GrossAmount;
        var profit = agencyCost.HasValue ? -(net - agencyCost.Value) : (decimal?)null;

        var tx = new FinancialTransaction(
            payment.TenantId,
            payment.Id,
            payment.QuoteId,
            Domain.Enums.FinancialTransactionType.Chargeback,
            payment.GrossAmount,
            payment.FeeAmount,
            net,
            agencyCost,
            profit,
            DateTime.UtcNow,
            "Chargeback registrado",
            createdBy);

        await transactionRepository.AddAsync(tx, cancellationToken);
    }
}
