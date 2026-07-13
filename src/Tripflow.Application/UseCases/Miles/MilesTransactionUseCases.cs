using FluentValidation;
using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Miles;
using Tripflow.Application.Helpers;
using Tripflow.Application.Services.Miles;
using Tripflow.Application.UseCases.Miles.Interfaces;
using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Miles;

public sealed class GetMilesTransactionsUseCase(
    ICustomerLoyaltyAccountRepository customerLoyaltyAccountRepository,
    IMilesTransactionRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetMilesTransactionsUseCase
{
    public async Task<Result<PagedResponse<MilesTransactionResponse>>> ExecuteAsync(Guid accountId, MilesTransactionFilterRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<PagedResponse<MilesTransactionResponse>>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesRead, cancellationToken))
            return Result<PagedResponse<MilesTransactionResponse>>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var account = await customerLoyaltyAccountRepository.GetByIdAndTenantAsync(accountId, tenantId, cancellationToken);
        if (account is null)
            return Result<PagedResponse<MilesTransactionResponse>>.Failure("Conta de fidelidade não encontrada.");

        var filter = request.ToExpression(accountId);
        var orderBy = MilesTransactionOrderByHelper.Build(request.SortBy);
        var paged = await repository.GetPagedAsync(filter, request.Page, request.PageSize, orderBy, request.SortDesc);
        var mapped = paged.Items.Select(ToResponse).ToList();

        return Result<PagedResponse<MilesTransactionResponse>>.Ok(
            new PagedResponse<MilesTransactionResponse>(mapped, paged.Page, paged.PageSize, paged.TotalItems, paged.TotalPages));
    }

    private static MilesTransactionResponse ToResponse(MilesTransaction transaction)
        => new(
            transaction.Id,
            transaction.TenantId,
            transaction.CustomerLoyaltyAccountId,
            transaction.Type,
            transaction.Amount,
            transaction.CostPerThousand,
            transaction.TotalCost,
            transaction.Description,
            transaction.TransactionDateUtc,
            transaction.CreatedAtUtc);
}

public sealed class GetMilesTransactionByIdUseCase(
    ICustomerLoyaltyAccountRepository customerLoyaltyAccountRepository,
    IMilesTransactionRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetMilesTransactionByIdUseCase
{
    public async Task<Result<MilesTransactionResponse?>> ExecuteAsync(Guid accountId, Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<MilesTransactionResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesRead, cancellationToken))
            return Result<MilesTransactionResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var account = await customerLoyaltyAccountRepository.GetByIdAndTenantAsync(accountId, tenantId, cancellationToken);
        if (account is null)
            return Result<MilesTransactionResponse?>.Failure("Conta de fidelidade não encontrada.");

        var transaction = await repository.GetByIdAndAccountAsync(id, accountId, tenantId, cancellationToken);
        if (transaction is null)
            return Result<MilesTransactionResponse?>.Failure("Transação de milhas não encontrada.");

        return Result<MilesTransactionResponse?>.Ok(
            new MilesTransactionResponse(
                transaction.Id,
                transaction.TenantId,
                transaction.CustomerLoyaltyAccountId,
                transaction.Type,
                transaction.Amount,
                transaction.CostPerThousand,
                transaction.TotalCost,
                transaction.Description,
                transaction.TransactionDateUtc,
                transaction.CreatedAtUtc));
    }
}

public sealed class CreateMilesTransactionUseCase(
    IValidator<CreateMilesTransactionRequest> validator,
    IMilesLedgerService milesLedgerService,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : ICreateMilesTransactionUseCase
{
    public async Task<Result<MilesTransactionResponse?>> ExecuteAsync(CreateMilesTransactionRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<MilesTransactionResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesWrite, cancellationToken))
            return Result<MilesTransactionResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<MilesTransactionResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        try
        {
            var transaction = await milesLedgerService.ProcessTransactionAsync(tenantId, request, createdBy, cancellationToken);
            return Result<MilesTransactionResponse?>.Ok(
                new MilesTransactionResponse(
                    transaction.Id,
                    transaction.TenantId,
                    transaction.CustomerLoyaltyAccountId,
                    transaction.Type,
                    transaction.Amount,
                    transaction.CostPerThousand,
                    transaction.TotalCost,
                    transaction.Description,
                    transaction.TransactionDateUtc,
                    transaction.CreatedAtUtc));
        }
        catch (Exception ex)
        {
            return Result<MilesTransactionResponse?>.Failure(ex.Message);
        }
    }
}

public sealed class GetGlobalMilesTransactionByIdUseCase(
    IMilesTransactionRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetGlobalMilesTransactionByIdUseCase
{
    public async Task<Result<MilesTransactionResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<MilesTransactionResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesRead, cancellationToken))
            return Result<MilesTransactionResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var transaction = await repository.GetByIdAndTenantAsync(id, tenantId, cancellationToken);
        if (transaction is null)
            return Result<MilesTransactionResponse?>.Failure("Transação de milhas não encontrada.");

        return Result<MilesTransactionResponse?>.Ok(
            new MilesTransactionResponse(
                transaction.Id,
                transaction.TenantId,
                transaction.CustomerLoyaltyAccountId,
                transaction.Type,
                transaction.Amount,
                transaction.CostPerThousand,
                transaction.TotalCost,
                transaction.Description,
                transaction.TransactionDateUtc,
                transaction.CreatedAtUtc));
    }
}

public sealed class DeleteMilesTransactionUseCase(
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IDeleteMilesTransactionUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid accountId, Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesWrite, cancellationToken))
            return Result<bool>.Forbidden();

        return Result<bool>.Failure("Transações de milhas não podem ser removidas. Utilize uma transação de ajuste.");
    }
}
