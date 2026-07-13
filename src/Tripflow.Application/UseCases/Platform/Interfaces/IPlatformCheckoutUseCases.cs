using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.DTOs.Responses.Subscriptions;

namespace Tripflow.Application.UseCases.Platform.Interfaces;

public interface IGetPublicSubscriptionPlansUseCase
{
    Task<Result<IReadOnlyList<PublicSubscriptionPlanResponse>>> ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface ICreatePlatformCheckoutUseCase
{
    Task<Result<CreatePlatformCheckoutResponse?>> ExecuteAsync(CreatePlatformCheckoutRequest request, CancellationToken cancellationToken = default);
}

public interface IGetPlatformCheckoutStatusUseCase
{
    Task<Result<PlatformCheckoutStatusResponse?>> ExecuteAsync(Guid checkoutSessionId, CancellationToken cancellationToken = default);
}

public interface IProcessPlatformCheckoutWebhookUseCase
{
    Task<Result<bool>> ExecuteAsync(string payload, IReadOnlyDictionary<string, string> headers, CancellationToken cancellationToken = default);
}

public interface IGetTenantEntitlementsUseCase
{
    Task<Result<TenantEntitlementsResponse?>> ExecuteAsync(CancellationToken cancellationToken = default);
}
