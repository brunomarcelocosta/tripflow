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

public sealed class UpdateTenantBrandingUseCase(
    ITenantBrandingRepository repository,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IValidator<UpdateTenantBrandingRequest> validator,
    IMapper mapper,
    ILogger<UpdateTenantBrandingUseCase> logger) : IUpdateTenantBrandingUseCase
{
    public string ClassName = nameof(UpdateTenantBrandingUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<TenantBrandingResponse>> ExecuteAsync(
        UpdateTenantBrandingRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<TenantBrandingResponse>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantBrandingResponse>.Failure("Tenant não resolvido para o usuário atual.");

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.SettingsManage,
                cancellationToken))
        {
            return Result<TenantBrandingResponse>.Forbidden();
        }

        var changedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                "{ClassName} | {Method} | Erro de validação | TenantId={TenantId}",
                ClassName, Method, tenantContext.TenantId);

            var firstError = validationResult.Errors.FirstOrDefault();

            return Result<TenantBrandingResponse>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            var tenantId = tenantContext.TenantId.Value;

            var entity = await repository.GetTrackedByTenantIdAsync(tenantId, cancellationToken);

            if (entity is null)
            {
                entity = new TenantBranding(
                    tenantId,
                    logoUrl: null,
                    primaryColor: request.PrimaryColor,
                    secondaryColor: request.SecondaryColor,
                    textColor: request.TextColor,
                    proposalFooter: request.ProposalFooter,
                    createdBy: changedBy);

                await repository.AddAsync(entity, cancellationToken);
            }
            else
            {
                entity.UpdateColorsAndFooter(
                    request.PrimaryColor,
                    request.SecondaryColor,
                    request.TextColor,
                    request.ProposalFooter,
                    changedBy);

                await repository.UpdateAsync(entity, cancellationToken);
            }

            await repository.CommitTransactionAsync(cancellationToken);

            logger.LogInformation(
                "{ClassName} | {Method} | Branding atualizado com sucesso | TenantId={TenantId}",
                ClassName, Method, tenantId);

            var response = mapper.Map<TenantBrandingResponse>(entity);

            return Result<TenantBrandingResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "{ClassName} | {Method} | Erro inesperado ao atualizar branding | TenantId={TenantId} | {Message}",
                ClassName, Method, tenantContext.TenantId, ex.Message);

            await repository.RollbackTransactionAsync(cancellationToken);

            return Result<TenantBrandingResponse>.Failure("Erro inesperado ao atualizar branding.");
        }
    }
}
