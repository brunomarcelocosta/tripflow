using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Miles;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Miles.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Miles;

public sealed class GetCustomerMilesSummaryUseCase(
    ICustomerRepository customerRepository,
    ICustomerLoyaltyAccountRepository customerLoyaltyAccountRepository,
    IMilesExpirationBatchRepository milesExpirationBatchRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetCustomerMilesSummaryUseCase
{
    public async Task<Result<CustomerMilesSummaryResponse>> ExecuteAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<CustomerMilesSummaryResponse>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesRead, cancellationToken))
            return Result<CustomerMilesSummaryResponse>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var customer = await customerRepository.GetByIdAndTenantAsync(customerId, tenantId, cancellationToken);
        if (customer is null)
            return Result<CustomerMilesSummaryResponse>.Failure("Cliente não encontrado.");

        var accounts = await customerLoyaltyAccountRepository.GetByCustomerIdAsync(customerId, tenantId, cancellationToken);
        var summaries = new List<CustomerLoyaltyAccountSummaryResponse>();

        foreach (var account in accounts)
            summaries.Add(await MilesSummaryHelper.BuildAccountSummaryAsync(account, tenantId, milesExpirationBatchRepository, cancellationToken));

        return Result<CustomerMilesSummaryResponse>.Ok(
            MilesSummaryHelper.BuildCustomerSummary(customer.Id, customer.FullName, summaries));
    }
}

public sealed class GetMilesSummaryUseCase(
    ICustomerLoyaltyAccountRepository customerLoyaltyAccountRepository,
    IMilesExpirationBatchRepository milesExpirationBatchRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetMilesSummaryUseCase
{
    public async Task<Result<IReadOnlyCollection<CustomerMilesSummaryResponse>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<IReadOnlyCollection<CustomerMilesSummaryResponse>>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesRead, cancellationToken))
            return Result<IReadOnlyCollection<CustomerMilesSummaryResponse>>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var accounts = await customerLoyaltyAccountRepository.GetSummaryAccountsAsync(tenantId, cancellationToken);

        var grouped = new Dictionary<Guid, (string CustomerName, List<CustomerLoyaltyAccountSummaryResponse> Accounts)>();
        foreach (var account in accounts)
        {
            var summary = await MilesSummaryHelper.BuildAccountSummaryAsync(account, tenantId, milesExpirationBatchRepository, cancellationToken);
            if (!grouped.TryGetValue(account.CustomerId, out var entry))
            {
                entry = (account.Customer.FullName, []);
                grouped[account.CustomerId] = entry;
            }

            entry.Accounts.Add(summary);
            grouped[account.CustomerId] = entry;
        }

        var response = grouped
            .Select(g => MilesSummaryHelper.BuildCustomerSummary(g.Key, g.Value.CustomerName, g.Value.Accounts))
            .OrderBy(x => x.CustomerName)
            .ToList();

        return Result<IReadOnlyCollection<CustomerMilesSummaryResponse>>.Ok(response);
    }
}
