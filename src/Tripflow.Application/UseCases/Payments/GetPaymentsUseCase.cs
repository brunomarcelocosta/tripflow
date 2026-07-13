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

public class GetPaymentsUseCase(
    IPaymentRepository repository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetPaymentsUseCase
{
    public async Task<Result<PagedResponse<PaymentResponse>>> ExecuteAsync(PaymentFilterRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<PagedResponse<PaymentResponse>>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.PaymentsRead, cancellationToken))
            return Result<PagedResponse<PaymentResponse>>.Forbidden();

        var filter = request.ToExpression();
        var orderBy = PaymentOrderByHelper.Build(request.SortBy);
        var paged = await repository.GetPagedAsync(filter, request.Page, request.PageSize, orderBy, request.SortDesc, x => x.Links);

        var mapped = paged.Items.Select(mapper.Map<PaymentResponse>).ToList();
        return Result<PagedResponse<PaymentResponse>>.Ok(new PagedResponse<PaymentResponse>(mapped, paged.Page, paged.PageSize, paged.TotalItems, paged.TotalPages));
    }
}
