using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.DTOs.Responses.Tenants;
using Tripflow.Application.UseCases.Tenants.Interfaces;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Tenants;

public sealed class UpdateTenantFinancialSettingsUseCase(
    ITenantCommercialSettingsRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IValidator<UpdateTenantFinancialSettingsRequest> validator,
    IMapper mapper,
    ILogger<UpdateTenantFinancialSettingsUseCase> logger) : IUpdateTenantFinancialSettingsUseCase
{
    public string ClassName = nameof(UpdateTenantFinancialSettingsUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<TenantFinancialSettingsResponse>> ExecuteAsync(
        UpdateTenantFinancialSettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<TenantFinancialSettingsResponse>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantFinancialSettingsResponse>.Failure("Tenant não resolvido para o usuário atual.");

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.SettingsManage,
                cancellationToken))
        {
            return Result<TenantFinancialSettingsResponse>.Forbidden();
        }

        var changedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                "{ClassName} | {Method} | Erro de validação | TenantId={TenantId}",
                ClassName, Method, tenantContext.TenantId);

            var firstError = validationResult.Errors.FirstOrDefault();

            return Result<TenantFinancialSettingsResponse>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            var tenantId = tenantContext.TenantId.Value;

            var entity = await repository.GetTrackedByTenantIdAsync(tenantId, cancellationToken);

            if (entity is null)
            {
                entity = new TenantCommercialSettings(tenantId, changedBy);

                entity.UpdateFinancialDefaults(
                    request.DefaultProfitAmount,
                    request.DefaultProfitPercentage,
                    request.DefaultPixDiscountPercentage,
                    changedBy);

                await repository.AddAsync(entity, cancellationToken);
            }
            else
            {
                entity.UpdateFinancialDefaults(
                    request.DefaultProfitAmount,
                    request.DefaultProfitPercentage,
                    request.DefaultPixDiscountPercentage,
                    changedBy);

                await repository.UpdateAsync(entity, cancellationToken);
            }

            await repository.CommitTransactionAsync(cancellationToken);

            logger.LogInformation(
                "{ClassName} | {Method} | Configurações financeiras atualizadas com sucesso | TenantId={TenantId}",
                ClassName, Method, tenantId);

            var response = mapper.Map<TenantFinancialSettingsResponse>(entity);

            return Result<TenantFinancialSettingsResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "{ClassName} | {Method} | Erro inesperado ao atualizar configurações financeiras | TenantId={TenantId} | {Message}",
                ClassName, Method, tenantContext.TenantId, ex.Message);

            await repository.RollbackTransactionAsync(cancellationToken);

            return Result<TenantFinancialSettingsResponse>.Failure("Erro inesperado ao atualizar configurações financeiras.");
        }
    }
}
