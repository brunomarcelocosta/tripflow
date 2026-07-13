using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Miles;

namespace Tripflow.Application.UseCases.Miles.Interfaces;

public interface IGetLoyaltyProgramsUseCase
{
    Task<Result<PagedResponse<LoyaltyProgramResponse>>> ExecuteAsync(LoyaltyProgramFilterRequest request, CancellationToken cancellationToken = default);
}

public interface IGetLoyaltyProgramByIdUseCase
{
    Task<Result<LoyaltyProgramResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICreateLoyaltyProgramUseCase
{
    Task<Result<LoyaltyProgramResponse?>> ExecuteAsync(CreateLoyaltyProgramRequest request, CancellationToken cancellationToken = default);
}

public interface IUpdateLoyaltyProgramUseCase
{
    Task<Result<LoyaltyProgramResponse?>> ExecuteAsync(Guid id, UpdateLoyaltyProgramRequest request, CancellationToken cancellationToken = default);
}

public interface IActivateLoyaltyProgramUseCase
{
    Task<Result<LoyaltyProgramResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IInactivateLoyaltyProgramUseCase
{
    Task<Result<LoyaltyProgramResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IDeleteLoyaltyProgramUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IGetCustomerLoyaltyAccountsUseCase
{
    Task<Result<PagedResponse<CustomerLoyaltyAccountResponse>>> ExecuteAsync(Guid customerId, CustomerLoyaltyAccountFilterRequest request, CancellationToken cancellationToken = default);
}

public interface IGetCustomerLoyaltyAccountByIdUseCase
{
    Task<Result<CustomerLoyaltyAccountResponse?>> ExecuteAsync(Guid customerId, Guid id, CancellationToken cancellationToken = default);
}

public interface IGetLoyaltyAccountByIdUseCase
{
    Task<Result<CustomerLoyaltyAccountResponse?>> ExecuteAsync(Guid accountId, CancellationToken cancellationToken = default);
}

public interface ICreateCustomerLoyaltyAccountUseCase
{
    Task<Result<CustomerLoyaltyAccountResponse?>> ExecuteAsync(Guid customerId, CreateCustomerLoyaltyAccountRequest request, CancellationToken cancellationToken = default);
}

public interface IUpdateCustomerLoyaltyAccountUseCase
{
    Task<Result<CustomerLoyaltyAccountResponse?>> ExecuteAsync(Guid customerId, Guid id, UpdateCustomerLoyaltyAccountRequest request, CancellationToken cancellationToken = default);
}

public interface IDeleteCustomerLoyaltyAccountUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid customerId, Guid id, CancellationToken cancellationToken = default);
}

public interface IActivateCustomerLoyaltyAccountUseCase
{
    Task<Result<CustomerLoyaltyAccountResponse?>> ExecuteAsync(Guid customerId, Guid id, CancellationToken cancellationToken = default);
}

public interface IInactivateCustomerLoyaltyAccountUseCase
{
    Task<Result<CustomerLoyaltyAccountResponse?>> ExecuteAsync(Guid customerId, Guid id, CancellationToken cancellationToken = default);
}

public interface ISuspendCustomerLoyaltyAccountUseCase
{
    Task<Result<CustomerLoyaltyAccountResponse?>> ExecuteAsync(Guid customerId, Guid id, CancellationToken cancellationToken = default);
}

public interface IGetMilesTransactionsUseCase
{
    Task<Result<PagedResponse<MilesTransactionResponse>>> ExecuteAsync(Guid accountId, MilesTransactionFilterRequest request, CancellationToken cancellationToken = default);
}

public interface IGetMilesTransactionByIdUseCase
{
    Task<Result<MilesTransactionResponse?>> ExecuteAsync(Guid accountId, Guid id, CancellationToken cancellationToken = default);
}

public interface IGetGlobalMilesTransactionByIdUseCase
{
    Task<Result<MilesTransactionResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICreateMilesTransactionUseCase
{
    Task<Result<MilesTransactionResponse?>> ExecuteAsync(CreateMilesTransactionRequest request, CancellationToken cancellationToken = default);
}

public interface IDeleteMilesTransactionUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid accountId, Guid id, CancellationToken cancellationToken = default);
}

public interface IGetMilesExpirationBatchesUseCase
{
    Task<Result<PagedResponse<MilesExpirationBatchResponse>>> ExecuteAsync(Guid accountId, MilesExpirationBatchFilterRequest request, CancellationToken cancellationToken = default);
}

public interface ICreateMilesExpirationBatchUseCase
{
    Task<Result<MilesExpirationBatchResponse?>> ExecuteAsync(CreateMilesExpirationBatchRequest request, CancellationToken cancellationToken = default);
}

public interface IUpdateMilesExpirationBatchUseCase
{
    Task<Result<MilesExpirationBatchResponse?>> ExecuteAsync(Guid accountId, Guid id, UpdateMilesExpirationBatchRequest request, CancellationToken cancellationToken = default);
}

public interface ICancelMilesExpirationBatchUseCase
{
    Task<Result<MilesExpirationBatchResponse?>> ExecuteAsync(Guid accountId, Guid id, CancellationToken cancellationToken = default);
}

public interface IMarkMilesExpirationBatchAsExpiredUseCase
{
    Task<Result<MilesExpirationBatchResponse?>> ExecuteAsync(Guid accountId, Guid id, CancellationToken cancellationToken = default);
}

public interface IGetCustomerMilesSummaryUseCase
{
    Task<Result<CustomerMilesSummaryResponse>> ExecuteAsync(Guid customerId, CancellationToken cancellationToken = default);
}

public interface IGetMilesSummaryUseCase
{
    Task<Result<IReadOnlyCollection<CustomerMilesSummaryResponse>>> ExecuteAsync(CancellationToken cancellationToken = default);
}
