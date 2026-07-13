using AutoMapper;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Tenants;
using Tripflow.Application.UseCases.Tenants.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Tenants;

public sealed class GetCurrentTenantCommercialSettingsUseCase(
    ITenantCommercialSettingsRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IMapper mapper,
    ILogger<GetCurrentTenantCommercialSettingsUseCase> logger) : IGetCurrentTenantCommercialSettingsUseCase
{
    public string ClassName = nameof(GetCurrentTenantCommercialSettingsUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<TenantCommercialSettingsResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<TenantCommercialSettingsResponse>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantCommercialSettingsResponse>.Failure("Tenant não resolvido para o usuário atual.");

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.SettingsRead,
                cancellationToken))
        {
            return Result<TenantCommercialSettingsResponse>.Forbidden();
        }

        try
        {
            var tenantId = tenantContext.TenantId.Value;

            var entity = await repository.GetByTenantIdAsync(tenantId, cancellationToken);

            if (entity is null)
            {
                logger.LogInformation(
                    "{ClassName} | {Method} | Configurações comerciais ainda não criadas | TenantId={TenantId}",
                    ClassName, Method, tenantId);

                return Result<TenantCommercialSettingsResponse>.Ok(new TenantCommercialSettingsResponse
                {
                    TenantId = tenantId,
                    DefaultProposalExpirationHours = 24
                });
            }

            var response = mapper.Map<TenantCommercialSettingsResponse>(entity);

            return Result<TenantCommercialSettingsResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "{ClassName} | {Method} | Erro inesperado ao consultar configurações comerciais | TenantId={TenantId} | {Message}",
                ClassName, Method, tenantContext.TenantId, ex.Message);

            return Result<TenantCommercialSettingsResponse>.Failure("Erro inesperado ao consultar configurações comerciais.");
        }
    }
}
