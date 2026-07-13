using Microsoft.Extensions.Logging;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.Application.Services.Subscriptions;

public interface ITenantUsageService
{
    const string Users = "users";
    const string Quotes = "quotes";

    Task<bool> IncrementAsync(string usageType, int amount = 1, CancellationToken cancellationToken = default);
    Task<bool> DecrementAsync(string usageType, int amount = 1, CancellationToken cancellationToken = default);
    Task<bool> HasAvailableLimitAsync(string usageType, CancellationToken cancellationToken = default);
    Task<TenantUsage?> GetCurrentUsageAsync(string usageType, CancellationToken cancellationToken = default);
}

public sealed class TenantUsageService(
    ITenantContext tenantContext,
    ITenantUsageRepository tenantUsageRepository,
    ITenantSubscriptionRepository tenantSubscriptionRepository,
    ILogger<TenantUsageService> logger) : ITenantUsageService
{
    public async Task<bool> IncrementAsync(string usageType, int amount = 1, CancellationToken cancellationToken = default)
    {
        if (!TryGetTenantId(out var tenantId))
            return false;

        var (year, month) = GetCurrentPeriod();
        var normalizedType = NormalizeUsageType(usageType);
        var limit = await ResolveLimitAsync(tenantId, normalizedType, cancellationToken);

        var usage = await tenantUsageRepository.GetTrackedCurrentByTypeAsync(tenantId, normalizedType, year, month, cancellationToken);
        if (usage is null)
        {
            usage = new TenantUsage(tenantId, normalizedType, year, month, 0, limit);
            usage.Increment(amount);
            await tenantUsageRepository.AddAsync(usage, cancellationToken);
            return true;
        }

        usage.SetLimit(limit);
        usage.Increment(amount);
        await tenantUsageRepository.UpdateAsync(usage, cancellationToken);
        return true;
    }

    public async Task<bool> DecrementAsync(string usageType, int amount = 1, CancellationToken cancellationToken = default)
    {
        if (!TryGetTenantId(out var tenantId))
            return false;

        var (year, month) = GetCurrentPeriod();
        var normalizedType = NormalizeUsageType(usageType);
        var usage = await tenantUsageRepository.GetTrackedCurrentByTypeAsync(tenantId, normalizedType, year, month, cancellationToken);
        if (usage is null)
            return true;

        usage.Decrement(amount);
        await tenantUsageRepository.UpdateAsync(usage, cancellationToken);
        return true;
    }

    public async Task<bool> HasAvailableLimitAsync(string usageType, CancellationToken cancellationToken = default)
    {
        if (!TryGetTenantId(out var tenantId))
            return false;

        var (year, month) = GetCurrentPeriod();
        var normalizedType = NormalizeUsageType(usageType);
        var limit = await ResolveLimitAsync(tenantId, normalizedType, cancellationToken);
        if (!limit.HasValue)
            return true;

        var usage = await tenantUsageRepository.GetCurrentByTypeAsync(tenantId, normalizedType, year, month, cancellationToken);
        if (usage is null)
            return true;

        return usage.CurrentValue < limit.Value;
    }

    public async Task<TenantUsage?> GetCurrentUsageAsync(string usageType, CancellationToken cancellationToken = default)
    {
        if (!TryGetTenantId(out var tenantId))
            return null;

        var (year, month) = GetCurrentPeriod();
        var normalizedType = NormalizeUsageType(usageType);
        var usage = await tenantUsageRepository.GetCurrentByTypeAsync(tenantId, normalizedType, year, month, cancellationToken);
        if (usage is not null)
            return usage;

        var limit = await ResolveLimitAsync(tenantId, normalizedType, cancellationToken);
        return new TenantUsage(tenantId, normalizedType, year, month, 0, limit);
    }

    private bool TryGetTenantId(out Guid tenantId)
    {
        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
        {
            tenantId = Guid.Empty;
            logger.LogWarning("TenantUsageService | Tenant não resolvido para a operação de consumo.");
            return false;
        }

        tenantId = tenantContext.TenantId.Value;
        return true;
    }

    private static (int year, int month) GetCurrentPeriod()
    {
        var now = DateTime.UtcNow;
        return (now.Year, now.Month);
    }

    private async Task<int?> ResolveLimitAsync(Guid tenantId, string usageType, CancellationToken cancellationToken)
    {
        var subscription = await tenantSubscriptionRepository.GetByTenantIdAsync(tenantId, cancellationToken);
        if (subscription?.SubscriptionPlan is null)
            return null;

        return usageType switch
        {
            ITenantUsageService.Users => subscription.SubscriptionPlan.MaxUsers,
            ITenantUsageService.Quotes => subscription.SubscriptionPlan.MaxQuotesPerMonth,
            _ => null
        };
    }

    private static string NormalizeUsageType(string usageType)
        => usageType.Trim().ToLowerInvariant();
}
