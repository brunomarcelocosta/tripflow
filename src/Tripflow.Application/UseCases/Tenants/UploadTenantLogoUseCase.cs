using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.DTOs.Responses.Tenants;
using Tripflow.Application.UseCases.Tenants.Interfaces;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Domain.Interfaces.Storage;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Tenants;

public sealed class UploadTenantLogoUseCase(
    ITenantBrandingRepository repository,
    IFileStorageService fileStorageService,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    IValidator<UploadTenantLogoRequest> validator,
    ILogger<UploadTenantLogoUseCase> logger) : IUploadTenantLogoUseCase
{
    public string ClassName = nameof(UploadTenantLogoUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<UploadTenantLogoResponse>> ExecuteAsync(
        UploadTenantLogoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<UploadTenantLogoResponse>.Forbidden();

        if (!tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<UploadTenantLogoResponse>.Failure("Tenant não resolvido para o usuário atual.");

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.SettingsManage,
                cancellationToken))
        {
            return Result<UploadTenantLogoResponse>.Forbidden();
        }

        var changedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                "{ClassName} | {Method} | Erro de validação no upload de logo | TenantId={TenantId}",
                ClassName, Method, tenantContext.TenantId);

            var firstError = validationResult.Errors.FirstOrDefault();

            return Result<UploadTenantLogoResponse>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        var tenantId = tenantContext.TenantId.Value;

        string logoUrl;

        try
        {
            var folder = $"tenants/{tenantId}/branding";

            logoUrl = await fileStorageService.UploadAsync(
                request.Content,
                folder,
                request.FileName,
                request.ContentType,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "{ClassName} | {Method} | Erro inesperado ao salvar logo | TenantId={TenantId} | {Message}",
                ClassName, Method, tenantId, ex.Message);

            return Result<UploadTenantLogoResponse>.Failure("Erro inesperado ao salvar o logo da empresa.");
        }

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            var entity = await repository.GetTrackedByTenantIdAsync(tenantId, cancellationToken);

            if (entity is null)
            {
                entity = new TenantBranding(
                    tenantId,
                    logoUrl: logoUrl,
                    primaryColor: null,
                    secondaryColor: null,
                    textColor: null,
                    proposalFooter: null,
                    createdBy: changedBy);

                await repository.AddAsync(entity, cancellationToken);
            }
            else
            {
                entity.UpdateLogo(logoUrl, changedBy);
                await repository.UpdateAsync(entity, cancellationToken);
            }

            await repository.CommitTransactionAsync(cancellationToken);

            logger.LogInformation(
                "{ClassName} | {Method} | Logo do tenant atualizado com sucesso | TenantId={TenantId}",
                ClassName, Method, tenantId);

            return Result<UploadTenantLogoResponse>.Ok(new UploadTenantLogoResponse
            {
                TenantId = tenantId,
                LogoUrl = logoUrl
            });
        }
        catch (Exception ex)
        {
            logger.LogError(
                "{ClassName} | {Method} | Erro inesperado ao persistir logo | TenantId={TenantId} | {Message}",
                ClassName, Method, tenantId, ex.Message);

            await repository.RollbackTransactionAsync(cancellationToken);

            return Result<UploadTenantLogoResponse>.Failure("Erro inesperado ao atualizar o logo da empresa.");
        }
    }
}
