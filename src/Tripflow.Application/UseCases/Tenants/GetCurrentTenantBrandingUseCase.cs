using AutoMapper;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Tenants;
using Tripflow.Application.UseCases.Tenants.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Tenants;

public sealed class GetCurrentTenantBrandingUseCase(
    ITenantBrandingRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IMapper mapper,
    ILogger<GetCurrentTenantBrandingUseCase> logger) : IGetCurrentTenantBrandingUseCase
{
    public string ClassName = nameof(GetCurrentTenantBrandingUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<TenantBrandingResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<TenantBrandingResponse>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantBrandingResponse>.Failure("Tenant não resolvido para o usuário atual.");

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.SettingsRead,
                cancellationToken))
        {
            return Result<TenantBrandingResponse>.Forbidden();
        }

        try
        {
            var tenantId = tenantContext.TenantId.Value;

            var entity = await repository.GetByTenantIdAsync(tenantId, cancellationToken);

            if (entity is null)
            {
                logger.LogInformation(
                    "{ClassName} | {Method} | Branding ainda não configurado | TenantId={TenantId}",
                    ClassName, Method, tenantId);

                return Result<TenantBrandingResponse>.Ok(new TenantBrandingResponse { TenantId = tenantId });
            }

            var response = mapper.Map<TenantBrandingResponse>(entity);

            return Result<TenantBrandingResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "{ClassName} | {Method} | Erro inesperado ao consultar branding | TenantId={TenantId} | {Message}",
                ClassName, Method, tenantContext.TenantId, ex.Message);

            return Result<TenantBrandingResponse>.Failure("Erro inesperado ao consultar branding.");
        }
    }
}
