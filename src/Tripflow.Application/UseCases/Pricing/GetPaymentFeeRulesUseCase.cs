using AutoMapper;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Pricing;
using Tripflow.Application.UseCases.Pricing.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Pricing;

public sealed class GetPaymentFeeRulesUseCase(
    IPaymentFeeRuleRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IMapper mapper,
    ILogger<GetPaymentFeeRulesUseCase> logger) : IGetPaymentFeeRulesUseCase
{
    public string ClassName = nameof(GetPaymentFeeRulesUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<List<PaymentFeeRuleResponse>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<List<PaymentFeeRuleResponse>>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<List<PaymentFeeRuleResponse>>.Failure("Tenant não resolvido para o usuário atual.");

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.SettingsRead,
                cancellationToken))
        {
            return Result<List<PaymentFeeRuleResponse>>.Forbidden();
        }

        try
        {
            var tenantId = tenantContext.TenantId.Value;

            var rules = await repository.GetByTenantIdAsync(tenantId, cancellationToken);

            var response = mapper.Map<List<PaymentFeeRuleResponse>>(rules);

            return Result<List<PaymentFeeRuleResponse>>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "{ClassName} | {Method} | Erro inesperado ao consultar regras de taxa | TenantId={TenantId} | {Message}",
                ClassName, Method, tenantContext.TenantId, ex.Message);

            return Result<List<PaymentFeeRuleResponse>>.Failure("Erro inesperado ao consultar regras de taxa.");
        }
    }
}
