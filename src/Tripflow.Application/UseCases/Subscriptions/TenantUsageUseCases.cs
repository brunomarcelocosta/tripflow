using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Subscriptions;
using Tripflow.Application.Services.Subscriptions;
using Tripflow.Application.UseCases.Subscriptions.Interfaces;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Subscriptions;

public sealed class GetCurrentTenantUsagesUseCase(
    ITenantUsageRepository repository,
    ITenantUsageService tenantUsageService,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetCurrentTenantUsagesUseCase
{
    public async Task<Result<IEnumerable<TenantUsageResponse>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<IEnumerable<TenantUsageResponse>>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsRead, cancellationToken))
            return Result<IEnumerable<TenantUsageResponse>>.Forbidden();

        var now = DateTime.UtcNow;
        var list = await repository.GetCurrentByTenantAsync(tenantContext.TenantId.Value, now.Year, now.Month, cancellationToken);

        var users = await tenantUsageService.GetCurrentUsageAsync(ITenantUsageService.Users, cancellationToken);
        var quotes = await tenantUsageService.GetCurrentUsageAsync(ITenantUsageService.Quotes, cancellationToken);

        if (users is not null && list.All(x => x.UsageType != users.UsageType))
            list.Add(users);
        if (quotes is not null && list.All(x => x.UsageType != quotes.UsageType))
            list.Add(quotes);

        return Result<IEnumerable<TenantUsageResponse>>.Ok(list
            .OrderBy(x => x.UsageType)
            .Select(TenantUsageMapping.Map)
            .ToList());
    }
}

public sealed class GetTenantUsageByTypeUseCase(
    ITenantUsageService tenantUsageService,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetTenantUsageByTypeUseCase
{
    public async Task<Result<TenantUsageResponse?>> ExecuteAsync(string usageType, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantUsageResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsRead, cancellationToken))
            return Result<TenantUsageResponse?>.Forbidden();

        if (string.IsNullOrWhiteSpace(usageType))
            return Result<TenantUsageResponse?>.Failure("Tipo de consumo é obrigatório.");

        var usage = await tenantUsageService.GetCurrentUsageAsync(usageType, cancellationToken);
        return Result<TenantUsageResponse?>.Ok(usage is null ? null : TenantUsageMapping.Map(usage));
    }
}

public sealed class IncrementTenantUsageUseCase(
    ITenantUsageService tenantUsageService,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IIncrementTenantUsageUseCase
{
    public async Task<Result<TenantUsageResponse?>> ExecuteAsync(string usageType, int amount, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantUsageResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<TenantUsageResponse?>.Forbidden();

        if (string.IsNullOrWhiteSpace(usageType))
            return Result<TenantUsageResponse?>.Failure("Tipo de consumo é obrigatório.");

        if (amount <= 0)
            return Result<TenantUsageResponse?>.Failure("A quantidade deve ser maior que zero.");

        var incremented = await tenantUsageService.IncrementAsync(usageType, amount, cancellationToken);
        if (!incremented)
            return Result<TenantUsageResponse?>.Failure("Não foi possível incrementar o consumo.");

        var usage = await tenantUsageService.GetCurrentUsageAsync(usageType, cancellationToken);
        return Result<TenantUsageResponse?>.Ok(usage is null ? null : TenantUsageMapping.Map(usage));
    }
}

public sealed class DecrementTenantUsageUseCase(
    ITenantUsageService tenantUsageService,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IDecrementTenantUsageUseCase
{
    public async Task<Result<TenantUsageResponse?>> ExecuteAsync(string usageType, int amount, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantUsageResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<TenantUsageResponse?>.Forbidden();

        if (string.IsNullOrWhiteSpace(usageType))
            return Result<TenantUsageResponse?>.Failure("Tipo de consumo é obrigatório.");

        if (amount <= 0)
            return Result<TenantUsageResponse?>.Failure("A quantidade deve ser maior que zero.");

        var decremented = await tenantUsageService.DecrementAsync(usageType, amount, cancellationToken);
        if (!decremented)
            return Result<TenantUsageResponse?>.Failure("Não foi possível decrementar o consumo.");

        var usage = await tenantUsageService.GetCurrentUsageAsync(usageType, cancellationToken);
        return Result<TenantUsageResponse?>.Ok(usage is null ? null : TenantUsageMapping.Map(usage));
    }
}

internal static class TenantUsageMapping
{
    public static TenantUsageResponse Map(TenantUsage usage)
        => new()
        {
            Id = usage.Id,
            TenantId = usage.TenantId,
            UsageType = usage.UsageType,
            PeriodYear = usage.PeriodYear,
            PeriodMonth = usage.PeriodMonth,
            CurrentValue = usage.CurrentValue,
            LimitValue = usage.LimitValue,
            UpdatedAtUtc = usage.UpdatedAtUtc
        };
}
