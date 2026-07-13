using AutoMapper;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Tenants;
using Tripflow.Application.UseCases.Tenants.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Tenants;

public sealed class GetCurrentTenantFinancialSettingsUseCase(
    ITenantCommercialSettingsRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IMapper mapper,
    ILogger<GetCurrentTenantFinancialSettingsUseCase> logger) : IGetCurrentTenantFinancialSettingsUseCase
{
    public string ClassName = nameof(GetCurrentTenantFinancialSettingsUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<TenantFinancialSettingsResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<TenantFinancialSettingsResponse>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantFinancialSettingsResponse>.Failure("Tenant não resolvido para o usuário atual.");

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.SettingsRead,
                cancellationToken))
        {
            return Result<TenantFinancialSettingsResponse>.Forbidden();
        }

        try
        {
            var tenantId = tenantContext.TenantId.Value;

            var entity = await repository.GetByTenantIdAsync(tenantId, cancellationToken);

            if (entity is null)
            {
                logger.LogInformation(
                    "{ClassName} | {Method} | Configurações financeiras ainda não criadas | TenantId={TenantId}",
                    ClassName, Method, tenantId);

                return Result<TenantFinancialSettingsResponse>.Ok(new TenantFinancialSettingsResponse
                {
                    TenantId = tenantId
                });
            }

            var response = mapper.Map<TenantFinancialSettingsResponse>(entity);

            return Result<TenantFinancialSettingsResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "{ClassName} | {Method} | Erro inesperado ao consultar configurações financeiras | TenantId={TenantId} | {Message}",
                ClassName, Method, tenantContext.TenantId, ex.Message);

            return Result<TenantFinancialSettingsResponse>.Failure("Erro inesperado ao consultar configurações financeiras.");
        }
    }
}
