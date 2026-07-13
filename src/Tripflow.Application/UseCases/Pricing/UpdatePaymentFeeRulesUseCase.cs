using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Pricing;
using Tripflow.Application.DTOs.Responses.Pricing;
using Tripflow.Application.UseCases.Pricing.Interfaces;
using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Pricing;

public sealed class UpdatePaymentFeeRulesUseCase(
    IPaymentFeeRuleRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IValidator<UpdatePaymentFeeRulesRequest> validator,
    IMapper mapper,
    ILogger<UpdatePaymentFeeRulesUseCase> logger) : IUpdatePaymentFeeRulesUseCase
{
    public string ClassName = nameof(UpdatePaymentFeeRulesUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<List<PaymentFeeRuleResponse>>> ExecuteAsync(
        UpdatePaymentFeeRulesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<List<PaymentFeeRuleResponse>>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<List<PaymentFeeRuleResponse>>.Failure("Tenant não resolvido para o usuário atual.");

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.SettingsManage,
                cancellationToken))
        {
            return Result<List<PaymentFeeRuleResponse>>.Forbidden();
        }

        var changedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                "{ClassName} | {Method} | Erro de validação | TenantId={TenantId}",
                ClassName, Method, tenantContext.TenantId);

            var firstError = validationResult.Errors.FirstOrDefault();

            return Result<List<PaymentFeeRuleResponse>>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            var tenantId = tenantContext.TenantId.Value;

            foreach (var item in request.Rules)
            {
                var existing = await repository.GetTrackedByTenantPaymentMethodAndInstallmentsAsync(
                    tenantId,
                    item.PaymentMethod,
                    item.Installments,
                    cancellationToken);

                if (existing is null)
                {
                    var rule = new PaymentFeeRule(
                        tenantId,
                        item.PaymentMethod,
                        item.Installments,
                        item.FeePercentage,
                        item.IsActive,
                        changedBy);

                    await repository.AddAsync(rule, cancellationToken);
                }
                else
                {
                    existing.Update(item.FeePercentage, item.IsActive, changedBy);
                    await repository.UpdateAsync(existing, cancellationToken);
                }
            }

            await repository.CommitTransactionAsync(cancellationToken);

            logger.LogInformation(
                "{ClassName} | {Method} | Regras de taxa atualizadas com sucesso | TenantId={TenantId}",
                ClassName, Method, tenantId);

            var rules = await repository.GetByTenantIdAsync(tenantId, cancellationToken);
            var response = mapper.Map<List<PaymentFeeRuleResponse>>(rules);

            return Result<List<PaymentFeeRuleResponse>>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "{ClassName} | {Method} | Erro inesperado ao atualizar regras de taxa | TenantId={TenantId} | {Message}",
                ClassName, Method, tenantContext.TenantId, ex.Message);

            await repository.RollbackTransactionAsync(cancellationToken);

            return Result<List<PaymentFeeRuleResponse>>.Failure("Erro inesperado ao atualizar regras de taxa.");
        }
    }
}
