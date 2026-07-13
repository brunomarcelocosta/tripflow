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

public sealed class UpdateTenantCommercialSettingsUseCase(
    ITenantCommercialSettingsRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IValidator<UpdateTenantCommercialSettingsRequest> validator,
    IMapper mapper,
    ILogger<UpdateTenantCommercialSettingsUseCase> logger) : IUpdateTenantCommercialSettingsUseCase
{
    public string ClassName = nameof(UpdateTenantCommercialSettingsUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<TenantCommercialSettingsResponse>> ExecuteAsync(
        UpdateTenantCommercialSettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<TenantCommercialSettingsResponse>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantCommercialSettingsResponse>.Failure("Tenant não resolvido para o usuário atual.");

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.SettingsManage,
                cancellationToken))
        {
            return Result<TenantCommercialSettingsResponse>.Forbidden();
        }

        var changedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                "{ClassName} | {Method} | Erro de validação | TenantId={TenantId}",
                ClassName, Method, tenantContext.TenantId);

            var firstError = validationResult.Errors.FirstOrDefault();

            return Result<TenantCommercialSettingsResponse>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            var tenantId = tenantContext.TenantId.Value;

            var entity = await repository.GetTrackedByTenantIdAsync(tenantId, cancellationToken);

            if (entity is null)
            {
                entity = new TenantCommercialSettings(tenantId, changedBy);

                entity.UpdateCommercialData(
                    request.CommercialEmail,
                    request.CommercialPhone,
                    request.WhatsApp,
                    request.Instagram,
                    request.Website,
                    request.DefaultTerms,
                    request.DefaultImportantNotes,
                    request.DefaultProposalExpirationHours,
                    changedBy);

                await repository.AddAsync(entity, cancellationToken);
            }
            else
            {
                entity.UpdateCommercialData(
                    request.CommercialEmail,
                    request.CommercialPhone,
                    request.WhatsApp,
                    request.Instagram,
                    request.Website,
                    request.DefaultTerms,
                    request.DefaultImportantNotes,
                    request.DefaultProposalExpirationHours,
                    changedBy);

                await repository.UpdateAsync(entity, cancellationToken);
            }

            await repository.CommitTransactionAsync(cancellationToken);

            logger.LogInformation(
                "{ClassName} | {Method} | Configurações comerciais atualizadas com sucesso | TenantId={TenantId}",
                ClassName, Method, tenantId);

            var response = mapper.Map<TenantCommercialSettingsResponse>(entity);

            return Result<TenantCommercialSettingsResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "{ClassName} | {Method} | Erro inesperado ao atualizar configurações comerciais | TenantId={TenantId} | {Message}",
                ClassName, Method, tenantContext.TenantId, ex.Message);

            await repository.RollbackTransactionAsync(cancellationToken);

            return Result<TenantCommercialSettingsResponse>.Failure("Erro inesperado ao atualizar configurações comerciais.");
        }
    }
}
