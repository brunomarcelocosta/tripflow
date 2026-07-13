using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Payments;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Payments;

namespace Tripflow.Application.UseCases.Payments.Interfaces;

public interface IGetPaymentProvidersUseCase
{
    Task<Result<IEnumerable<PaymentProviderResponse>>> ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface IGetTenantPaymentProvidersUseCase
{
    Task<Result<IEnumerable<TenantPaymentProviderResponse>>> ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface ICreateTenantPaymentProviderUseCase
{
    Task<Result<TenantPaymentProviderResponse?>> ExecuteAsync(CreateTenantPaymentProviderRequest request, CancellationToken cancellationToken = default);
}

public interface IUpdateTenantPaymentProviderUseCase
{
    Task<Result<TenantPaymentProviderResponse?>> ExecuteAsync(Guid id, UpdateTenantPaymentProviderRequest request, CancellationToken cancellationToken = default);
}

public interface IDeleteTenantPaymentProviderUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ISetDefaultTenantPaymentProviderUseCase
{
    Task<Result<TenantPaymentProviderResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IActivateTenantPaymentProviderUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IInactivateTenantPaymentProviderUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IGetPaymentsUseCase
{
    Task<Result<PagedResponse<PaymentResponse>>> ExecuteAsync(PaymentFilterRequest request, CancellationToken cancellationToken = default);
}

public interface IGetPaymentByIdUseCase
{
    Task<Result<PaymentResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICreatePaymentFromProposalUseCase
{
    Task<Result<PaymentResponse?>> ExecuteAsync(Guid proposalId, CreatePaymentFromProposalRequest request, CancellationToken cancellationToken = default);
}

public interface IMarkPaymentAsPaidUseCase
{
    Task<Result<PaymentResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICancelPaymentUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IRefundPaymentUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IGetPaymentLinksUseCase
{
    Task<Result<IEnumerable<PaymentLinkResponse>>> ExecuteAsync(Guid paymentId, CancellationToken cancellationToken = default);
}

public interface ICreatePaymentLinkUseCase
{
    Task<Result<PaymentLinkResponse?>> ExecuteAsync(Guid paymentId, CreatePaymentLinkRequest request, CancellationToken cancellationToken = default);
}

public interface ICancelPaymentLinkUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid paymentId, Guid linkId, CancellationToken cancellationToken = default);
}

public interface IReceivePaymentWebhookUseCase
{
    Task<Result<bool>> ExecuteAsync(string providerCode, string payload, CancellationToken cancellationToken = default);
}

public interface IProcessPaymentWebhookUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid webhookEventId, CancellationToken cancellationToken = default);
}

public interface IGetFinancialTransactionsUseCase
{
    Task<Result<PagedResponse<FinancialTransactionResponse>>> ExecuteAsync(FinancialTransactionFilterRequest request, CancellationToken cancellationToken = default);
}

public interface IGetFinancialTransactionByIdUseCase
{
    Task<Result<FinancialTransactionResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}
