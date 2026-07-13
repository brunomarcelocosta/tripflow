using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Payments;
using Tripflow.Application.DTOs.Responses.Payments;
using Tripflow.Domain.Interfaces.Security;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Payments;

public class UpdateTenantPaymentProviderUseCase(
    ITenantPaymentProviderRepository repository,
    ISecretProtector secretProtector,
    IValidator<UpdateTenantPaymentProviderRequest> validator,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<UpdateTenantPaymentProviderUseCase> logger) : IUpdateTenantPaymentProviderUseCase
{
    public async Task<Result<TenantPaymentProviderResponse?>> ExecuteAsync(Guid id, UpdateTenantPaymentProviderRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantPaymentProviderResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<TenantPaymentProviderResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<TenantPaymentProviderResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var entity = await repository.GetTrackedByIdAndTenantAsync(id, tenantId, cancellationToken);
        if (entity is null)
            return Result<TenantPaymentProviderResponse?>.Failure("Provedor de pagamento não encontrado.");

        try
        {
            if (request.IsDefault && !entity.IsDefault)
            {
                var others = await repository.GetTrackedByTenantIdAsync(tenantId, cancellationToken);
                foreach (var item in others.Where(x => x.IsDefault && x.Id != id))
                    item.SetDefault(false, updatedBy);
            }

            var encryptedApiKey = !string.IsNullOrWhiteSpace(request.ApiKey) ? secretProtector.Protect(request.ApiKey) : null;
            var encryptedSecret = !string.IsNullOrWhiteSpace(request.Secret) ? secretProtector.Protect(request.Secret) : null;

            entity.Update(
                request.DisplayName,
                encryptedApiKey,
                encryptedSecret,
                request.WebhookSecret,
                request.IsDefault,
                request.Status,
                updatedBy);

            await repository.UpdateAsync(entity, cancellationToken);

            var fresh = await repository.GetByIdAndTenantAsync(id, tenantId, cancellationToken);
            return Result<TenantPaymentProviderResponse?>.Ok(mapper.Map<TenantPaymentProviderResponse>(fresh));
        }
        catch (Exception ex)
        {
            logger.LogError("UpdateTenantPaymentProviderUseCase | Erro | {Message}", ex.Message);
            return Result<TenantPaymentProviderResponse?>.Failure("Erro inesperado ao atualizar provedor de pagamento.");
        }
    }
}
