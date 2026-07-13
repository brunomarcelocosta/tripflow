using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Subscriptions;

namespace Tripflow.Application.UseCases.Subscriptions.Interfaces;

public interface IGetSubscriptionPlansUseCase
{
    Task<Result<PagedResponse<SubscriptionPlanResponse>>> ExecuteAsync(SubscriptionPlanFilterRequest request, CancellationToken cancellationToken = default);
}

public interface IGetSubscriptionPlanByIdUseCase
{
    Task<Result<SubscriptionPlanResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICreateSubscriptionPlanUseCase
{
    Task<Result<SubscriptionPlanResponse?>> ExecuteAsync(CreateSubscriptionPlanRequest request, CancellationToken cancellationToken = default);
}

public interface IUpdateSubscriptionPlanUseCase
{
    Task<Result<SubscriptionPlanResponse?>> ExecuteAsync(Guid id, UpdateSubscriptionPlanRequest request, CancellationToken cancellationToken = default);
}

public interface IDeleteSubscriptionPlanUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IActivateSubscriptionPlanUseCase
{
    Task<Result<SubscriptionPlanResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IInactivateSubscriptionPlanUseCase
{
    Task<Result<SubscriptionPlanResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IDeprecateSubscriptionPlanUseCase
{
    Task<Result<SubscriptionPlanResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IGetPlanFeaturesUseCase
{
    Task<Result<IEnumerable<PlanFeatureResponse>>> ExecuteAsync(Guid planId, CancellationToken cancellationToken = default);
}

public interface IUpdatePlanFeaturesUseCase
{
    Task<Result<IEnumerable<PlanFeatureResponse>>> ExecuteAsync(Guid planId, UpdatePlanFeaturesRequest request, CancellationToken cancellationToken = default);
}

public interface IGetCurrentTenantSubscriptionUseCase
{
    Task<Result<TenantSubscriptionResponse?>> ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface IUpdateTenantSubscriptionUseCase
{
    Task<Result<TenantSubscriptionResponse?>> ExecuteAsync(UpdateTenantSubscriptionRequest request, CancellationToken cancellationToken = default);
}

public interface IActivateTenantSubscriptionUseCase
{
    Task<Result<TenantSubscriptionResponse?>> ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface ISuspendTenantSubscriptionUseCase
{
    Task<Result<TenantSubscriptionResponse?>> ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface ICancelTenantSubscriptionUseCase
{
    Task<Result<TenantSubscriptionResponse?>> ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface IMarkTenantSubscriptionPastDueUseCase
{
    Task<Result<TenantSubscriptionResponse?>> ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface IGetCurrentTenantUsagesUseCase
{
    Task<Result<IEnumerable<TenantUsageResponse>>> ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface IGetTenantUsageByTypeUseCase
{
    Task<Result<TenantUsageResponse?>> ExecuteAsync(string usageType, CancellationToken cancellationToken = default);
}

public interface IIncrementTenantUsageUseCase
{
    Task<Result<TenantUsageResponse?>> ExecuteAsync(string usageType, int amount, CancellationToken cancellationToken = default);
}

public interface IDecrementTenantUsageUseCase
{
    Task<Result<TenantUsageResponse?>> ExecuteAsync(string usageType, int amount, CancellationToken cancellationToken = default);
}
