using Tripflow.Domain.Entities.Payments;

namespace Tripflow.Domain.Interfaces;

public interface IPaymentProviderRepository : IBaseRepository<PaymentProvider>
{
    Task<List<PaymentProvider>> GetActiveAsync(CancellationToken cancellationToken = default);

    Task<PaymentProvider?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
