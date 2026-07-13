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
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Miles;

public sealed class GetMilesExpirationBatchesUseCase(
    ICustomerLoyaltyAccountRepository customerLoyaltyAccountRepository,
    IMilesExpirationBatchRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetMilesExpirationBatchesUseCase
{
    public async Task<Result<PagedResponse<MilesExpirationBatchResponse>>> ExecuteAsync(Guid accountId, MilesExpirationBatchFilterRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<PagedResponse<MilesExpirationBatchResponse>>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesRead, cancellationToken))
            return Result<PagedResponse<MilesExpirationBatchResponse>>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var account = await customerLoyaltyAccountRepository.GetByIdAndTenantAsync(accountId, tenantId, cancellationToken);
        if (account is null)
            return Result<PagedResponse<MilesExpirationBatchResponse>>.Failure("Conta de fidelidade não encontrada.");

        var filter = request.ToExpression(accountId);
        var orderBy = MilesExpirationBatchOrderByHelper.Build(request.SortBy);
        var paged = await repository.GetPagedAsync(filter, request.Page, request.PageSize, orderBy, request.SortDesc);
        var mapped = paged.Items.Select(ToResponse).ToList();

        return Result<PagedResponse<MilesExpirationBatchResponse>>.Ok(
            new PagedResponse<MilesExpirationBatchResponse>(mapped, paged.Page, paged.PageSize, paged.TotalItems, paged.TotalPages));
    }

    private static MilesExpirationBatchResponse ToResponse(MilesExpirationBatch batch)
        => new(batch.Id, batch.TenantId, batch.CustomerLoyaltyAccountId, batch.Amount, batch.RemainingAmount, batch.ExpiresAt, batch.Status, batch.CreatedAtUtc, batch.UpdatedAtUtc);
}

public sealed class CreateMilesExpirationBatchUseCase(
    ICustomerLoyaltyAccountRepository customerLoyaltyAccountRepository,
    IMilesExpirationBatchRepository repository,
    IValidator<CreateMilesExpirationBatchRequest> validator,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : ICreateMilesExpirationBatchUseCase
{
    public async Task<Result<MilesExpirationBatchResponse?>> ExecuteAsync(CreateMilesExpirationBatchRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<MilesExpirationBatchResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesWrite, cancellationToken))
            return Result<MilesExpirationBatchResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<MilesExpirationBatchResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var account = await customerLoyaltyAccountRepository.GetByIdAndTenantAsync(request.CustomerLoyaltyAccountId, tenantId, cancellationToken);
        if (account is null)
            return Result<MilesExpirationBatchResponse?>.Failure("Conta de fidelidade não encontrada.");

        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        var batch = new MilesExpirationBatch(
            tenantId,
            request.CustomerLoyaltyAccountId,
            request.Amount,
            request.ExpiresAt,
            request.Status ?? MilesExpirationStatus.Pending,
            createdBy);

        await repository.AddAsync(batch, cancellationToken);
        return Result<MilesExpirationBatchResponse?>.Ok(ToResponse(batch));
    }

    private static MilesExpirationBatchResponse ToResponse(MilesExpirationBatch batch)
        => new(batch.Id, batch.TenantId, batch.CustomerLoyaltyAccountId, batch.Amount, batch.RemainingAmount, batch.ExpiresAt, batch.Status, batch.CreatedAtUtc, batch.UpdatedAtUtc);
}

public sealed class UpdateMilesExpirationBatchUseCase(
    IMilesExpirationBatchRepository repository,
    IValidator<UpdateMilesExpirationBatchRequest> validator,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IUpdateMilesExpirationBatchUseCase
{
    public async Task<Result<MilesExpirationBatchResponse?>> ExecuteAsync(Guid accountId, Guid id, UpdateMilesExpirationBatchRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<MilesExpirationBatchResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesWrite, cancellationToken))
            return Result<MilesExpirationBatchResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<MilesExpirationBatchResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var batch = await repository.GetTrackedByIdAndAccountAsync(id, accountId, tenantId, cancellationToken);
        if (batch is null)
            return Result<MilesExpirationBatchResponse?>.Failure("Lote de expiração não encontrado.");

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        try
        {
            batch.Update(request.Amount, request.ExpiresAt, updatedBy);
        }
        catch (Exception ex)
        {
            return Result<MilesExpirationBatchResponse?>.Failure(ex.Message);
        }

        await repository.UpdateAsync(batch, cancellationToken);
        return Result<MilesExpirationBatchResponse?>.Ok(ToResponse(batch));
    }

    private static MilesExpirationBatchResponse ToResponse(MilesExpirationBatch batch)
        => new(batch.Id, batch.TenantId, batch.CustomerLoyaltyAccountId, batch.Amount, batch.RemainingAmount, batch.ExpiresAt, batch.Status, batch.CreatedAtUtc, batch.UpdatedAtUtc);
}

public sealed class CancelMilesExpirationBatchUseCase(
    IMilesExpirationBatchRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : ICancelMilesExpirationBatchUseCase
{
    public async Task<Result<MilesExpirationBatchResponse?>> ExecuteAsync(Guid accountId, Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<MilesExpirationBatchResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesWrite, cancellationToken))
            return Result<MilesExpirationBatchResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var batch = await repository.GetTrackedByIdAndAccountAsync(id, accountId, tenantId, cancellationToken);
        if (batch is null)
            return Result<MilesExpirationBatchResponse?>.Failure("Lote de expiração não encontrado.");

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        try
        {
            batch.Cancel(updatedBy);
        }
        catch (Exception ex)
        {
            return Result<MilesExpirationBatchResponse?>.Failure(ex.Message);
        }

        await repository.UpdateAsync(batch, cancellationToken);
        return Result<MilesExpirationBatchResponse?>.Ok(ToResponse(batch));
    }

    private static MilesExpirationBatchResponse ToResponse(MilesExpirationBatch batch)
        => new(batch.Id, batch.TenantId, batch.CustomerLoyaltyAccountId, batch.Amount, batch.RemainingAmount, batch.ExpiresAt, batch.Status, batch.CreatedAtUtc, batch.UpdatedAtUtc);
}

public sealed class MarkMilesExpirationBatchAsExpiredUseCase(
    IMilesExpirationBatchRepository repository,
    IMilesLedgerService milesLedgerService,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IMarkMilesExpirationBatchAsExpiredUseCase
{
    public async Task<Result<MilesExpirationBatchResponse?>> ExecuteAsync(Guid accountId, Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<MilesExpirationBatchResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesWrite, cancellationToken))
            return Result<MilesExpirationBatchResponse?>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var batch = await repository.GetTrackedByIdAndAccountAsync(id, accountId, tenantId, cancellationToken);
        if (batch is null)
            return Result<MilesExpirationBatchResponse?>.Failure("Lote de expiração não encontrado.");

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        var amountToExpire = batch.RemainingAmount;

        try
        {
            batch.MarkAsExpired(updatedBy);
            await repository.UpdateAsync(batch, cancellationToken);

            if (amountToExpire > 0)
            {
                var transactionRequest = new CreateMilesTransactionRequest(
                    accountId,
                    MilesTransactionType.Expiration,
                    amountToExpire,
                    null,
                    $"Expiração automática do lote {batch.Id}",
                    DateTime.UtcNow,
                    null,
                    false);

                await milesLedgerService.ProcessTransactionAsync(tenantId, transactionRequest, updatedBy, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            return Result<MilesExpirationBatchResponse?>.Failure(ex.Message);
        }

        return Result<MilesExpirationBatchResponse?>.Ok(
            new MilesExpirationBatchResponse(batch.Id, batch.TenantId, batch.CustomerLoyaltyAccountId, batch.Amount, batch.RemainingAmount, batch.ExpiresAt, batch.Status, batch.CreatedAtUtc, batch.UpdatedAtUtc));
    }
}
