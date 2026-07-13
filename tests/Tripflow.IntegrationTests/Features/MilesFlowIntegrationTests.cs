using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.Services.Miles;
using Tripflow.Application.Services.Subscriptions;
using Tripflow.Application.UseCases.Leads;
using Tripflow.Application.UseCases.Miles;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Data.Repositories.CRM;
using Tripflow.Infra.Data.Repositories.Miles;
using Tripflow.Infra.Data.Repositories.Subscriptions;
using Tripflow.Infra.Services;
using Tripflow.IntegrationTests.Utils;

namespace Tripflow.IntegrationTests.Features;

public class MilesFlowIntegrationTests
{
    [Fact]
    public async Task FullMilesFlow_Should_CreditDebitExpireAndSummarize()
    {
        var tenantContext = new TestTenantContext();
        var userContext = new TestUserContext();
        var permissionService = new TestUserPermissionService();
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();

        Guid tenantId;
        Guid customerId;
        Guid accountId;
        Guid programId;
        Guid batchId;

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var tenant = new Tenant("Agência Milhas", "Agência Milhas", null, "milhas@test.com", null, TenantStatus.Active, "system");
            seed.Tenants.Add(tenant);

            var customer = new Customer(tenant.Id, CustomerType.Person, "Cliente Milhas", null, "cliente@milhas.com", null, null, null, "system");
            seed.Customers.Add(customer);

            var program = new LoyaltyProgram("Smiles", "BR", "GOL", LoyaltyProgramStatus.Active, "system");
            seed.LoyaltyPrograms.Add(program);

            var account = new CustomerLoyaltyAccount(
                tenant.Id, customer.Id, program.Id, "123456", 0, 18m, null,
                LoyaltyAccountStatus.Active, "system");
            seed.CustomerLoyaltyAccounts.Add(account);

            await seed.SaveChangesAsync();
            tenantId = tenant.Id;
            customerId = customer.Id;
            accountId = account.Id;
            programId = program.Id;
            tenantContext.SetTenant(tenantId);
        }

        await using var context = TripflowDbContextTestFactory.Create(tenantContext, databaseName);

        var customerRepo = new CustomerRepository(context);
        var accountRepo = new CustomerLoyaltyAccountRepository(context);
        var batchRepo = new MilesExpirationBatchRepository(context);
        var transactionRepo = new MilesTransactionRepository(context);
        var ledger = new MilesLedgerService(transactionRepo, accountRepo, batchRepo);

        var createAccount = new CreateCustomerLoyaltyAccountUseCase(
            customerRepo,
            new LoyaltyProgramRepository(context),
            accountRepo,
            TestValidators.AlwaysValid<CreateCustomerLoyaltyAccountRequest>(),
            userContext, tenantContext, permissionService);

        var duplicate = await createAccount.ExecuteAsync(customerId, new CreateCustomerLoyaltyAccountRequest(
            programId, "123456", 0, 18m, "dup", null));
        Assert.False(duplicate.Success);

        await ledger.ProcessTransactionAsync(
            tenantId,
            new CreateMilesTransactionRequest(accountId, MilesTransactionType.Credit, 3000, 18m, "Entrada", DateTime.UtcNow, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(45)), true),
            "system");

        var createBatch = new CreateMilesExpirationBatchUseCase(
            accountRepo, batchRepo,
            TestValidators.AlwaysValid<CreateMilesExpirationBatchRequest>(),
            userContext, tenantContext, permissionService);
        var batchResult = await createBatch.ExecuteAsync(new CreateMilesExpirationBatchRequest(accountId, 1000, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(20)), null));
        Assert.True(batchResult.Success, batchResult.Error);
        batchId = batchResult.Data!.Id;

        await ledger.ProcessTransactionAsync(
            tenantId,
            new CreateMilesTransactionRequest(accountId, MilesTransactionType.Debit, 500, null, "Saída", DateTime.UtcNow, null, false),
            "system");

        var markExpired = new MarkMilesExpirationBatchAsExpiredUseCase(
            batchRepo, ledger, userContext, tenantContext, permissionService);
        var expireResult = await markExpired.ExecuteAsync(accountId, batchId);
        Assert.True(expireResult.Success, expireResult.Error);

        var summaryUseCase = new GetCustomerMilesSummaryUseCase(
            customerRepo, accountRepo, batchRepo, userContext, tenantContext, permissionService);
        var summary = await summaryUseCase.ExecuteAsync(customerId);
        Assert.True(summary.Success, summary.Error);
        Assert.True(summary.Data!.TotalBalance >= 0);
        Assert.NotEmpty(summary.Data.Accounts);

        var otherTenantContext = new TestTenantContext();
        otherTenantContext.SetTenant(Guid.NewGuid());
        var crossTenant = await accountRepo.GetByCustomerAndTenantAsync(accountId, customerId, otherTenantContext.TenantId!.Value);
        Assert.Null(crossTenant);
    }
}

public class MilesAndSubscriptionServiceIntegrationTests
{
    [Fact]
    public async Task MilesLedgerService_CreditAndDebit_Should_UpdateBalanceAndConsumeBatches()
    {
        var tenantContext = new TestTenantContext();
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantId;
        Guid accountId;

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var tenant = new Tenant("Tenant Milhas", "Tenant Milhas", null, "t@m.com", null, TenantStatus.Active, "system");
            seed.Tenants.Add(tenant);
            tenantId = tenant.Id;

            var customer = new Customer(tenantId, CustomerType.Person, "Cliente", null, "c@test.com", null, null, null, "system");
            seed.Customers.Add(customer);

            var program = new LoyaltyProgram("Smiles", "BR", "GOL", LoyaltyProgramStatus.Active, "system");
            seed.LoyaltyPrograms.Add(program);

            var account = new CustomerLoyaltyAccount(
                tenantId, customer.Id, program.Id, "ACC-1", 0, 20m, null,
                LoyaltyAccountStatus.Active, "system");
            seed.CustomerLoyaltyAccounts.Add(account);

            await seed.SaveChangesAsync();
            accountId = account.Id;
            tenantContext.SetTenant(tenantId);
        }

        await using var context = TripflowDbContextTestFactory.Create(tenantContext, databaseName);
        var transactionRepo = new MilesTransactionRepository(context);
        var accountRepo = new CustomerLoyaltyAccountRepository(context);
        var batchRepo = new MilesExpirationBatchRepository(context);
        var service = new MilesLedgerService(transactionRepo, accountRepo, batchRepo);

        await service.ProcessTransactionAsync(
            tenantId,
            new CreateMilesTransactionRequest(accountId, MilesTransactionType.Credit, 5000, 20m, "Crédito", DateTime.UtcNow, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(60)), true),
            "system");

        var trackedAccount = await accountRepo.GetByIdAndTenantAsync(accountId, tenantId);
        Assert.Equal(5000, trackedAccount!.CurrentBalance);

        await service.ProcessTransactionAsync(
            tenantId,
            new CreateMilesTransactionRequest(accountId, MilesTransactionType.Debit, 2000, null, "Débito", DateTime.UtcNow, null, false),
            "system");

        trackedAccount = await accountRepo.GetByIdAndTenantAsync(accountId, tenantId);
        Assert.Equal(3000, trackedAccount!.CurrentBalance);

        var batches = await batchRepo.GetByAccountIdAsync(accountId, tenantId);
        Assert.Equal(3000, batches[0].RemainingAmount);
    }

    [Fact]
    public async Task TenantUsageService_Should_EnforceQuoteLimit()
    {
        var tenantContext = new TestTenantContext();
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid tenantId;
        Guid planId;

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            tenantId = Guid.NewGuid();
            var plan = new SubscriptionPlan("Starter", null, 99m, 999m, 2, 10, SubscriptionPlanStatus.Active, "system");
            seed.SubscriptionPlans.Add(plan);
            seed.TenantSubscriptions.Add(new TenantSubscription(
                tenantId, plan.Id, TenantSubscriptionStatus.Active, DateTime.UtcNow.AddDays(-1), null, null, "system"));
            seed.TenantUsages.Add(new TenantUsage(tenantId, ITenantUsageService.Quotes, DateTime.UtcNow.Year, DateTime.UtcNow.Month, 10, 10));

            await seed.SaveChangesAsync();
            planId = plan.Id;
            tenantContext.SetTenant(tenantId);
        }

        await using var context = TripflowDbContextTestFactory.Create(tenantContext, databaseName);
        var service = new TenantUsageService(
            tenantContext,
            new TenantUsageRepository(context),
            new TenantSubscriptionRepository(context),
            NullLogger<TenantUsageService>.Instance);

        Assert.False(await service.HasAvailableLimitAsync(ITenantUsageService.Quotes));
    }
}

public class SubscriptionLeadFlowIntegrationTests
{
    [Fact]
    public async Task ConvertLeadToTenant_Should_CreateTenantAndSubscription()
    {
        var userContext = new TestUserContext();
        var permissionService = new TestUserPermissionService();
        var databaseName = TripflowDbContextTestFactory.CreateDatabaseName();
        Guid leadId;
        Guid planId;

        await using (var seed = TripflowDbContextTestFactory.Create(databaseName: databaseName))
        {
            var plan = new SubscriptionPlan("Trial", "Plano trial", 0m, 0m, 3, 20, SubscriptionPlanStatus.Active, "system");
            seed.SubscriptionPlans.Add(plan);

            var lead = new Lead("Nova Empresa", "João", "nova@empresa.com", "11999999999", "Trial", "Quero testar", "public");
            seed.Leads.Add(lead);
            await seed.SaveChangesAsync();
            leadId = lead.Id;
            planId = plan.Id;
        }

        await using var context = TripflowDbContextTestFactory.Create(databaseName: databaseName);
        var tenantContext = new TestTenantContext();

        var useCase = new ConvertLeadToTenantUseCase(
            new LeadRepository(context),
            new Tripflow.Infra.Data.Repositories.Tenants.TenantRepository(context),
            new Tripflow.Infra.Data.Repositories.Tenants.TenantBrandingRepository(context),
            new Tripflow.Infra.Data.Repositories.Tenants.TenantCommercialSettingsRepository(context),
            new TenantSubscriptionRepository(context),
            new SubscriptionPlanRepository(context),
            new TenantRoleProvisioningService(context, new Tripflow.Infra.Data.Repositories.Identity.PermissionRepository(context)),
            userContext,
            permissionService,
            TestValidators.AlwaysValid<ConvertLeadToTenantRequest>(),
            NullLogger<ConvertLeadToTenantUseCase>.Instance);

        var result = await useCase.ExecuteAsync(leadId, new ConvertLeadToTenantRequest
        {
            LegalName = "Nova Empresa LTDA",
            TradeName = "Nova Empresa",
            DocumentNumber = "12345678000199",
            SubscriptionPlanId = planId
        });
        Assert.True(result.Success, result.Error);
        Assert.NotNull(result.Data);

        tenantContext.SetTenant(result.Data!.Value);
        await using var tenantScoped = TripflowDbContextTestFactory.Create(tenantContext, databaseName);

        var subscription = await tenantScoped.TenantSubscriptions.FirstOrDefaultAsync(x => x.TenantId == result.Data.Value);
        Assert.NotNull(subscription);
        Assert.Equal(planId, subscription!.SubscriptionPlanId);

        var convertedLead = await tenantScoped.Leads.FirstAsync(x => x.Id == leadId);
        Assert.True(convertedLead.ConvertedToTenant);
    }
}

internal sealed class TestUserContext : Tripflow.Domain.Interfaces.Contexts.IUserContext
{
    public bool IsAuthenticated => true;
    public string? Email => "integration@test.com";
    public string? IdentityProviderUserId => "integration-user";
    public string? Name => "Integration Test";
    public IReadOnlyList<string> Roles => [];
}

internal sealed class TestUserPermissionService : IUserPermissionService
{
    public Task<bool> HasPermissionAsync(string permissionCode, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public Task<IReadOnlyList<string>> GetCurrentUserPermissionsAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<string>>([]);
}

internal static class TestValidators
{
    public static IValidator<T> AlwaysValid<T>() => new InlineValidator<T>();
}
