using FluentValidation;
using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Miles;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Miles.Interfaces;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Miles;

public sealed class GetCustomerLoyaltyAccountsUseCase(
    ICustomerRepository customerRepository,
    ICustomerLoyaltyAccountRepository repository,
    IMilesExpirationBatchRepository milesExpirationBatchRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetCustomerLoyaltyAccountsUseCase
{
    public async Task<Result<PagedResponse<CustomerLoyaltyAccountResponse>>> ExecuteAsync(Guid customerId, CustomerLoyaltyAccountFilterRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<PagedResponse<CustomerLoyaltyAccountResponse>>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesRead, cancellationToken))
            return Result<PagedResponse<CustomerLoyaltyAccountResponse>>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var customer = await customerRepository.GetByIdAndTenantAsync(customerId, tenantId, cancellationToken);
        if (customer is null)
            return Result<PagedResponse<CustomerLoyaltyAccountResponse>>.Failure("Cliente não encontrado.");

        var filter = request.ToExpression(customerId);
        var orderBy = CustomerLoyaltyAccountOrderByHelper.Build(request.SortBy);
        var paged = await repository.GetPagedAsync(
            filter,
            request.Page,
            request.PageSize,
            orderBy,
            request.SortDesc,
            x => x.Customer,
            x => x.LoyaltyProgram);

        var items = new List<CustomerLoyaltyAccountResponse>();
        foreach (var account in paged.Items)
            items.Add(await ToResponseAsync(account, tenantId, cancellationToken));

        return Result<PagedResponse<CustomerLoyaltyAccountResponse>>.Ok(
            new PagedResponse<CustomerLoyaltyAccountResponse>(items, paged.Page, paged.PageSize, paged.TotalItems, paged.TotalPages));
    }

    private async Task<CustomerLoyaltyAccountResponse> ToResponseAsync(CustomerLoyaltyAccount account, Guid tenantId, CancellationToken cancellationToken)
    {
        var expiringMiles = await milesExpirationBatchRepository.GetExpiringAmountAsync(
            account.Id,
            tenantId,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(90)),
            cancellationToken);

        return new CustomerLoyaltyAccountResponse(
            account.Id,
            account.TenantId,
            account.CustomerId,
            account.Customer.FullName,
            account.LoyaltyProgramId,
            account.LoyaltyProgram.Name,
            account.AccountNumber,
            account.CurrentBalance,
            account.AverageCostPerThousand,
            account.LastBalanceUpdateUtc,
            account.Notes,
            account.Status,
            expiringMiles,
            account.CreatedAtUtc,
            account.UpdatedAtUtc);
    }
}

public sealed class GetCustomerLoyaltyAccountByIdUseCase(
    ICustomerRepository customerRepository,
    ICustomerLoyaltyAccountRepository repository,
    IMilesExpirationBatchRepository milesExpirationBatchRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetCustomerLoyaltyAccountByIdUseCase
{
    public async Task<Result<CustomerLoyaltyAccountResponse?>> ExecuteAsync(Guid customerId, Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<CustomerLoyaltyAccountResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesRead, cancellationToken))
            return Result<CustomerLoyaltyAccountResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var customer = await customerRepository.GetByIdAndTenantAsync(customerId, tenantId, cancellationToken);
        if (customer is null)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Cliente não encontrado.");

        var account = await repository.GetByCustomerAndTenantAsync(id, customerId, tenantId, cancellationToken);
        if (account is null)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Conta de fidelidade não encontrada.");

        var expiringMiles = await milesExpirationBatchRepository.GetExpiringAmountAsync(
            account.Id,
            tenantId,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(90)),
            cancellationToken);

        return Result<CustomerLoyaltyAccountResponse?>.Ok(
            new CustomerLoyaltyAccountResponse(
                account.Id,
                account.TenantId,
                account.CustomerId,
                account.Customer.FullName,
                account.LoyaltyProgramId,
                account.LoyaltyProgram.Name,
                account.AccountNumber,
                account.CurrentBalance,
                account.AverageCostPerThousand,
                account.LastBalanceUpdateUtc,
                account.Notes,
                account.Status,
                expiringMiles,
                account.CreatedAtUtc,
                account.UpdatedAtUtc));
    }
}

public sealed class GetLoyaltyAccountByIdUseCase(
    ICustomerLoyaltyAccountRepository repository,
    IMilesExpirationBatchRepository milesExpirationBatchRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetLoyaltyAccountByIdUseCase
{
    public async Task<Result<CustomerLoyaltyAccountResponse?>> ExecuteAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<CustomerLoyaltyAccountResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesRead, cancellationToken))
            return Result<CustomerLoyaltyAccountResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var account = await repository.GetByIdAndTenantAsync(accountId, tenantId, cancellationToken);
        if (account is null)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Conta de fidelidade não encontrada.");

        var expiringMiles = await milesExpirationBatchRepository.GetExpiringAmountAsync(
            account.Id,
            tenantId,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(90)),
            cancellationToken);

        return Result<CustomerLoyaltyAccountResponse?>.Ok(
            new CustomerLoyaltyAccountResponse(
                account.Id,
                account.TenantId,
                account.CustomerId,
                account.Customer.FullName,
                account.LoyaltyProgramId,
                account.LoyaltyProgram.Name,
                account.AccountNumber,
                account.CurrentBalance,
                account.AverageCostPerThousand,
                account.LastBalanceUpdateUtc,
                account.Notes,
                account.Status,
                expiringMiles,
                account.CreatedAtUtc,
                account.UpdatedAtUtc));
    }
}

public sealed class CreateCustomerLoyaltyAccountUseCase(
    ICustomerRepository customerRepository,
    ILoyaltyProgramRepository loyaltyProgramRepository,
    ICustomerLoyaltyAccountRepository repository,
    IValidator<CreateCustomerLoyaltyAccountRequest> validator,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : ICreateCustomerLoyaltyAccountUseCase
{
    public async Task<Result<CustomerLoyaltyAccountResponse?>> ExecuteAsync(Guid customerId, CreateCustomerLoyaltyAccountRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<CustomerLoyaltyAccountResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesWrite, cancellationToken))
            return Result<CustomerLoyaltyAccountResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<CustomerLoyaltyAccountResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var customer = await customerRepository.GetByIdAndTenantAsync(customerId, tenantId, cancellationToken);
        if (customer is null)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Cliente não encontrado.");

        var loyaltyProgram = await loyaltyProgramRepository.GetByIdAsync(request.LoyaltyProgramId);
        if (loyaltyProgram is null)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Programa de fidelidade não encontrado.");

        if (loyaltyProgram.Status != LoyaltyProgramStatus.Active)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Somente programas de fidelidade ativos podem ser vinculados.");

        var exists = await repository.ExistsByCustomerProgramAndAccountNumberAsync(
            tenantId,
            customerId,
            request.LoyaltyProgramId,
            request.AccountNumber,
            cancellationToken);
        if (exists)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Já existe uma conta para este programa e número informado.");

        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        var entity = new CustomerLoyaltyAccount(
            tenantId,
            customerId,
            request.LoyaltyProgramId,
            request.AccountNumber?.Trim(),
            request.CurrentBalance,
            request.AverageCostPerThousand,
            request.Notes?.Trim(),
            request.Status ?? LoyaltyAccountStatus.Active,
            createdBy);

        await repository.AddAsync(entity, cancellationToken);

        var saved = await repository.GetByCustomerAndTenantAsync(entity.Id, customerId, tenantId, cancellationToken);
        if (saved is null)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Erro ao recuperar conta de fidelidade criada.");

        return Result<CustomerLoyaltyAccountResponse?>.Ok(
            new CustomerLoyaltyAccountResponse(
                saved.Id,
                saved.TenantId,
                saved.CustomerId,
                saved.Customer.FullName,
                saved.LoyaltyProgramId,
                saved.LoyaltyProgram.Name,
                saved.AccountNumber,
                saved.CurrentBalance,
                saved.AverageCostPerThousand,
                saved.LastBalanceUpdateUtc,
                saved.Notes,
                saved.Status,
                0,
                saved.CreatedAtUtc,
                saved.UpdatedAtUtc));
    }
}

public sealed class UpdateCustomerLoyaltyAccountUseCase(
    ICustomerRepository customerRepository,
    ILoyaltyProgramRepository loyaltyProgramRepository,
    ICustomerLoyaltyAccountRepository repository,
    IMilesExpirationBatchRepository milesExpirationBatchRepository,
    IValidator<UpdateCustomerLoyaltyAccountRequest> validator,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IUpdateCustomerLoyaltyAccountUseCase
{
    public async Task<Result<CustomerLoyaltyAccountResponse?>> ExecuteAsync(Guid customerId, Guid id, UpdateCustomerLoyaltyAccountRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<CustomerLoyaltyAccountResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesWrite, cancellationToken))
            return Result<CustomerLoyaltyAccountResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<CustomerLoyaltyAccountResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var customer = await customerRepository.GetByIdAndTenantAsync(customerId, tenantId, cancellationToken);
        if (customer is null)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Cliente não encontrado.");

        var loyaltyProgram = await loyaltyProgramRepository.GetByIdAsync(request.LoyaltyProgramId);
        if (loyaltyProgram is null)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Programa de fidelidade não encontrado.");

        if (loyaltyProgram.Status != LoyaltyProgramStatus.Active)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Somente programas de fidelidade ativos podem ser vinculados.");

        var account = await repository.GetTrackedByCustomerAndTenantAsync(id, customerId, tenantId, cancellationToken);
        if (account is null)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Conta de fidelidade não encontrada.");

        var exists = await repository.ExistsByCustomerProgramAndAccountNumberExceptIdAsync(
            tenantId,
            customerId,
            request.LoyaltyProgramId,
            request.AccountNumber,
            id,
            cancellationToken);
        if (exists)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Já existe uma conta para este programa e número informado.");

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        account.Update(
            request.LoyaltyProgramId,
            request.AccountNumber?.Trim(),
            request.CurrentBalance,
            request.AverageCostPerThousand,
            request.Notes?.Trim(),
            request.Status,
            updatedBy);

        await repository.UpdateAsync(account, cancellationToken);

        var expiringMiles = await milesExpirationBatchRepository.GetExpiringAmountAsync(
            account.Id,
            tenantId,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(90)),
            cancellationToken);

        return Result<CustomerLoyaltyAccountResponse?>.Ok(
            new CustomerLoyaltyAccountResponse(
                account.Id,
                account.TenantId,
                account.CustomerId,
                account.Customer.FullName,
                account.LoyaltyProgramId,
                account.LoyaltyProgram.Name,
                account.AccountNumber,
                account.CurrentBalance,
                account.AverageCostPerThousand,
                account.LastBalanceUpdateUtc,
                account.Notes,
                account.Status,
                expiringMiles,
                account.CreatedAtUtc,
                account.UpdatedAtUtc));
    }
}

public sealed class DeleteCustomerLoyaltyAccountUseCase(
    ICustomerRepository customerRepository,
    ICustomerLoyaltyAccountRepository repository,
    IMilesTransactionRepository milesTransactionRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IDeleteCustomerLoyaltyAccountUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid customerId, Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesWrite, cancellationToken))
            return Result<bool>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var customer = await customerRepository.GetByIdAndTenantAsync(customerId, tenantId, cancellationToken);
        if (customer is null)
            return Result<bool>.Failure("Cliente não encontrado.");

        var account = await repository.GetTrackedByCustomerAndTenantAsync(id, customerId, tenantId, cancellationToken);
        if (account is null)
            return Result<bool>.Failure("Conta de fidelidade não encontrada.");

        var hasTransactions = await milesTransactionRepository.AnyAsync(
            x => x.TenantId == tenantId && x.CustomerLoyaltyAccountId == id,
            cancellationToken);
        if (hasTransactions)
            return Result<bool>.Failure("Não é possível remover a conta pois existem transações vinculadas.");

        var deletedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        account.SetDelete(deletedBy);
        await repository.UpdateAsync(account, cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public sealed class ActivateCustomerLoyaltyAccountUseCase(
    ICustomerRepository customerRepository,
    ICustomerLoyaltyAccountRepository repository,
    IMilesExpirationBatchRepository milesExpirationBatchRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IActivateCustomerLoyaltyAccountUseCase
{
    public async Task<Result<CustomerLoyaltyAccountResponse?>> ExecuteAsync(Guid customerId, Guid id, CancellationToken cancellationToken = default)
        => await ChangeStatusAsync(customerId, id, cancellationToken, LoyaltyAccountStatus.Active);

    private async Task<Result<CustomerLoyaltyAccountResponse?>> ChangeStatusAsync(Guid customerId, Guid id, CancellationToken cancellationToken, LoyaltyAccountStatus status)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<CustomerLoyaltyAccountResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesWrite, cancellationToken))
            return Result<CustomerLoyaltyAccountResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var customer = await customerRepository.GetByIdAndTenantAsync(customerId, tenantId, cancellationToken);
        if (customer is null)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Cliente não encontrado.");

        var account = await repository.GetTrackedByCustomerAndTenantAsync(id, customerId, tenantId, cancellationToken);
        if (account is null)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Conta de fidelidade não encontrada.");

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        if (status == LoyaltyAccountStatus.Active)
            account.Activate(updatedBy);

        await repository.UpdateAsync(account, cancellationToken);

        var expiringMiles = await milesExpirationBatchRepository.GetExpiringAmountAsync(account.Id, tenantId, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(90)), cancellationToken);
        return Result<CustomerLoyaltyAccountResponse?>.Ok(
            new CustomerLoyaltyAccountResponse(
                account.Id,
                account.TenantId,
                account.CustomerId,
                account.Customer.FullName,
                account.LoyaltyProgramId,
                account.LoyaltyProgram.Name,
                account.AccountNumber,
                account.CurrentBalance,
                account.AverageCostPerThousand,
                account.LastBalanceUpdateUtc,
                account.Notes,
                account.Status,
                expiringMiles,
                account.CreatedAtUtc,
                account.UpdatedAtUtc));
    }
}

public sealed class InactivateCustomerLoyaltyAccountUseCase(
    ICustomerRepository customerRepository,
    ICustomerLoyaltyAccountRepository repository,
    IMilesExpirationBatchRepository milesExpirationBatchRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IInactivateCustomerLoyaltyAccountUseCase
{
    public async Task<Result<CustomerLoyaltyAccountResponse?>> ExecuteAsync(Guid customerId, Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<CustomerLoyaltyAccountResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesWrite, cancellationToken))
            return Result<CustomerLoyaltyAccountResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var customer = await customerRepository.GetByIdAndTenantAsync(customerId, tenantId, cancellationToken);
        if (customer is null)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Cliente não encontrado.");

        var account = await repository.GetTrackedByCustomerAndTenantAsync(id, customerId, tenantId, cancellationToken);
        if (account is null)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Conta de fidelidade não encontrada.");

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        account.Inactivate(updatedBy);
        await repository.UpdateAsync(account, cancellationToken);

        var expiringMiles = await milesExpirationBatchRepository.GetExpiringAmountAsync(account.Id, tenantId, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(90)), cancellationToken);
        return Result<CustomerLoyaltyAccountResponse?>.Ok(
            new CustomerLoyaltyAccountResponse(
                account.Id,
                account.TenantId,
                account.CustomerId,
                account.Customer.FullName,
                account.LoyaltyProgramId,
                account.LoyaltyProgram.Name,
                account.AccountNumber,
                account.CurrentBalance,
                account.AverageCostPerThousand,
                account.LastBalanceUpdateUtc,
                account.Notes,
                account.Status,
                expiringMiles,
                account.CreatedAtUtc,
                account.UpdatedAtUtc));
    }
}

public sealed class SuspendCustomerLoyaltyAccountUseCase(
    ICustomerRepository customerRepository,
    ICustomerLoyaltyAccountRepository repository,
    IMilesExpirationBatchRepository milesExpirationBatchRepository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : ISuspendCustomerLoyaltyAccountUseCase
{
    public async Task<Result<CustomerLoyaltyAccountResponse?>> ExecuteAsync(Guid customerId, Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<CustomerLoyaltyAccountResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesWrite, cancellationToken))
            return Result<CustomerLoyaltyAccountResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var customer = await customerRepository.GetByIdAndTenantAsync(customerId, tenantId, cancellationToken);
        if (customer is null)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Cliente não encontrado.");

        var account = await repository.GetTrackedByCustomerAndTenantAsync(id, customerId, tenantId, cancellationToken);
        if (account is null)
            return Result<CustomerLoyaltyAccountResponse?>.Failure("Conta de fidelidade não encontrada.");

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        account.Suspend(updatedBy);
        await repository.UpdateAsync(account, cancellationToken);

        var expiringMiles = await milesExpirationBatchRepository.GetExpiringAmountAsync(account.Id, tenantId, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(90)), cancellationToken);
        return Result<CustomerLoyaltyAccountResponse?>.Ok(
            new CustomerLoyaltyAccountResponse(
                account.Id,
                account.TenantId,
                account.CustomerId,
                account.Customer.FullName,
                account.LoyaltyProgramId,
                account.LoyaltyProgram.Name,
                account.AccountNumber,
                account.CurrentBalance,
                account.AverageCostPerThousand,
                account.LastBalanceUpdateUtc,
                account.Notes,
                account.Status,
                expiringMiles,
                account.CreatedAtUtc,
                account.UpdatedAtUtc));
    }
}
