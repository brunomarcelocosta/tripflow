using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Payments;
using Tripflow.Application.DTOs.Responses.Payments;
using Tripflow.Domain.Interfaces.Security;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Payments;

public class CreateTenantPaymentProviderUseCase(
    IPaymentProviderRepository paymentProviderRepository,
    ITenantPaymentProviderRepository repository,
    ISecretProtector secretProtector,
    IValidator<CreateTenantPaymentProviderRequest> validator,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<CreateTenantPaymentProviderUseCase> logger) : ICreateTenantPaymentProviderUseCase
{
    public async Task<Result<TenantPaymentProviderResponse?>> ExecuteAsync(CreateTenantPaymentProviderRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantPaymentProviderResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<TenantPaymentProviderResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<TenantPaymentProviderResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var paymentProvider = await paymentProviderRepository.GetByIdAsync(request.PaymentProviderId);
        if (paymentProvider is null || paymentProvider.Status != PaymentProviderStatus.Active)
            return Result<TenantPaymentProviderResponse?>.Failure("Provedor de pagamento não encontrado ou inativo.");

        try
        {
            if (request.IsDefault)
            {
                var existing = await repository.GetTrackedByTenantIdAsync(tenantId, cancellationToken);
                foreach (var item in existing.Where(x => x.IsDefault))
                    item.SetDefault(false, createdBy);
            }

            var encryptedApiKey = !string.IsNullOrWhiteSpace(request.ApiKey) ? secretProtector.Protect(request.ApiKey) : null;
            var encryptedSecret = !string.IsNullOrWhiteSpace(request.Secret) ? secretProtector.Protect(request.Secret) : null;

            var entity = new TenantPaymentProvider(
                tenantId,
                request.PaymentProviderId,
                request.DisplayName,
                encryptedApiKey,
                encryptedSecret,
                request.WebhookSecret,
                request.IsDefault,
                PaymentProviderStatus.Active,
                createdBy);

            await repository.AddAsync(entity, cancellationToken);

            var fresh = await repository.GetByIdAndTenantAsync(entity.Id, tenantId, cancellationToken);
            return Result<TenantPaymentProviderResponse?>.Ok(mapper.Map<TenantPaymentProviderResponse>(fresh));
        }
        catch (Exception ex)
        {
            logger.LogError("CreateTenantPaymentProviderUseCase | Erro | {Message}", ex.Message);
            return Result<TenantPaymentProviderResponse?>.Failure("Erro inesperado ao criar provedor de pagamento.");
        }
    }
}
