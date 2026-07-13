using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Payments;
using Tripflow.Application.DTOs.Responses.Payments;
using Tripflow.Application.Options;
using Tripflow.Application.Services.Payments;
using Tripflow.Application.Services.Proposals;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Domain.Interfaces.Security;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Payments;

public class CreatePaymentLinkUseCase(
    IPaymentRepository paymentRepository,
    IProposalRepository proposalRepository,
    ITenantPaymentProviderRepository tenantPaymentProviderRepository,
    IPaymentLinkRepository linkRepository,
    IPaymentGatewayService gatewayService,
    ISecretProtector secretProtector,
    IProposalEventService proposalEventService,
    IOptions<InfinitePayOptions> infinitePayOptions,
    IValidator<CreatePaymentLinkRequest> validator,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<CreatePaymentLinkUseCase> logger) : ICreatePaymentLinkUseCase
{
    public async Task<Result<PaymentLinkResponse?>> ExecuteAsync(Guid paymentId, CreatePaymentLinkRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<PaymentLinkResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.PaymentsWrite, cancellationToken))
            return Result<PaymentLinkResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<PaymentLinkResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var payment = await paymentRepository.GetTrackedByIdAndTenantAsync(paymentId, tenantId, cancellationToken);
        if (payment is null)
            return Result<PaymentLinkResponse?>.Failure("Pagamento não encontrado.");

        if (payment.Status is PaymentStatus.Cancelled or PaymentStatus.Refunded or PaymentStatus.Paid)
            return Result<PaymentLinkResponse?>.Failure("Pagamento não pode receber link no status atual.");

        try
        {
            var (providerCode, apiKey) = await ResolveProviderCredentialsAsync(tenantId, payment.TenantPaymentProviderId, cancellationToken);
            var proposalContext = await BuildProposalContextAsync(payment, tenantId, cancellationToken);

            var gatewayResult = await gatewayService.CreatePaymentLinkAsync(
                new CreatePaymentLinkCommand(
                    tenantId,
                    payment.Id,
                    providerCode,
                    apiKey,
                    payment.GrossAmount,
                    payment.PaymentMethod,
                    payment.Installments,
                    request.ExpiresAtUtc,
                    proposalContext.Description,
                    proposalContext.CustomerName,
                    proposalContext.CustomerEmail,
                    proposalContext.CustomerPhone,
                    infinitePayOptions.Value.CheckoutSuccessUrl,
                    BuildTenantWebhookUrl(),
                    payment.ProposalId),
                cancellationToken);

            var externalPaymentId = gatewayResult.ExternalPaymentReference ?? gatewayResult.ExternalLinkId;
            payment.SetExternalPaymentId(externalPaymentId, createdBy);
            await paymentRepository.UpdateAsync(payment, cancellationToken);

            var link = new PaymentLink(
                tenantId,
                payment.Id,
                gatewayResult.ExternalLinkId,
                gatewayResult.Url,
                request.ExpiresAtUtc,
                PaymentLinkStatus.Active,
                createdBy);

            await linkRepository.AddAsync(link, cancellationToken);

            if (payment.ProposalId.HasValue)
            {
                await proposalEventService.RegisterAsync(
                    tenantId,
                    payment.ProposalId.Value,
                    ProposalEventType.PaymentLinkCreated,
                    "Link de pagamento criado",
                    cancellationToken: cancellationToken);
            }

            return Result<PaymentLinkResponse?>.Ok(MapLink(link));
        }
        catch (Exception ex)
        {
            logger.LogError("CreatePaymentLinkUseCase | Erro | {Message}", ex.Message);
            return Result<PaymentLinkResponse?>.Failure("Erro inesperado ao criar link de pagamento.");
        }
    }

    private async Task<(string ProviderCode, string? ApiKey)> ResolveProviderCredentialsAsync(
        Guid tenantId,
        Guid? tenantPaymentProviderId,
        CancellationToken cancellationToken)
    {
        if (!tenantPaymentProviderId.HasValue)
            return ("manual", null);

        var tenantProvider = await tenantPaymentProviderRepository.GetByIdAndTenantAsync(
            tenantPaymentProviderId.Value,
            tenantId,
            cancellationToken);

        if (tenantProvider?.PaymentProvider is null)
            return ("manual", null);

        string? apiKey = null;
        if (!string.IsNullOrWhiteSpace(tenantProvider.EncryptedApiKey))
        {
            try
            {
                apiKey = secretProtector.Unprotect(tenantProvider.EncryptedApiKey);
            }
            catch
            {
                apiKey = null;
            }
        }

        return (tenantProvider.PaymentProvider.Code, apiKey);
    }

    private async Task<(string? Description, string? CustomerName, string? CustomerEmail, string? CustomerPhone)> BuildProposalContextAsync(
        Payment payment,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (!payment.ProposalId.HasValue)
            return (null, null, null, null);

        var proposal = await proposalRepository.GetByIdAndTenantAsync(payment.ProposalId.Value, tenantId, cancellationToken);
        if (proposal is null)
            return (null, null, null, null);

        var tradeName = proposal.Tenant.TradeName ?? proposal.Tenant.LegalName;
        var description = $"Pagamento da proposta {proposal.ProposalNumber} - {tradeName}";
        var customer = proposal.Quote.Customer;

        return (
            description,
            customer?.FullName,
            customer?.Email,
            customer?.Phone);
    }

    private string BuildTenantWebhookUrl()
        => $"{infinitePayOptions.Value.ApiPublicBaseUrl.TrimEnd('/')}/api/webhooks/payments/infinitepay";

    private static PaymentLinkResponse MapLink(PaymentLink link)
        => new(
            link.Id,
            link.TenantId,
            link.PaymentId,
            link.ExternalLinkId,
            link.Url,
            link.ExpiresAtUtc,
            link.Status);
}
