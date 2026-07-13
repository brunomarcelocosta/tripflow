using AutoMapper;
using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Payments;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Payments;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Payments;

public class GetFinancialTransactionsUseCase(
    IFinancialTransactionRepository repository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetFinancialTransactionsUseCase
{
    public async Task<Result<PagedResponse<FinancialTransactionResponse>>> ExecuteAsync(FinancialTransactionFilterRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<PagedResponse<FinancialTransactionResponse>>.Forbidden();

        if (!await HasPaymentsOrReportsReadAsync(cancellationToken))
            return Result<PagedResponse<FinancialTransactionResponse>>.Forbidden();

        var filter = request.ToExpression();
        var orderBy = FinancialTransactionOrderByHelper.Build(request.SortBy);
        var paged = await repository.GetPagedAsync(filter, request.Page, request.PageSize, orderBy, request.SortDesc);

        var mapped = paged.Items.Select(mapper.Map<FinancialTransactionResponse>).ToList();
        return Result<PagedResponse<FinancialTransactionResponse>>.Ok(
            new PagedResponse<FinancialTransactionResponse>(mapped, paged.Page, paged.PageSize, paged.TotalItems, paged.TotalPages));
    }

    private async Task<bool> HasPaymentsOrReportsReadAsync(CancellationToken cancellationToken)
        => await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.PaymentsRead, cancellationToken)
           || await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.ReportsRead, cancellationToken);
}
