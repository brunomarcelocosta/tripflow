using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Payments;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Payments;

public class GetPaymentLinksUseCase(
    IPaymentRepository paymentRepository,
    IPaymentLinkRepository linkRepository,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService) : IGetPaymentLinksUseCase
{
    public async Task<Result<IEnumerable<PaymentLinkResponse>>> ExecuteAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<IEnumerable<PaymentLinkResponse>>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.PaymentsRead, cancellationToken))
            return Result<IEnumerable<PaymentLinkResponse>>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var payment = await paymentRepository.GetByIdAndTenantAsync(paymentId, tenantId, cancellationToken);
        if (payment is null)
            return Result<IEnumerable<PaymentLinkResponse>>.Failure("Pagamento não encontrado.");

        var links = await linkRepository.GetByPaymentIdAsync(paymentId, tenantId, cancellationToken);
        var mapped = links.Select(mapper.Map<PaymentLinkResponse>).ToList();
        return Result<IEnumerable<PaymentLinkResponse>>.Ok(mapped);
    }
}
