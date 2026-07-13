using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Payments;
using Tripflow.Application.DTOs.Responses.Payments;
using Tripflow.Application.Services.Payments;
using Tripflow.Application.UseCases.Payments.Interfaces;
using Tripflow.Domain.Entities.Payments;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Payments;

public class CreatePaymentFromProposalUseCase(
    IProposalRepository proposalRepository,
    ITenantPaymentProviderRepository tenantPaymentProviderRepository,
    IPaymentRepository paymentRepository,
    IValidator<CreatePaymentFromProposalRequest> validator,
    ICreatePaymentLinkUseCase createPaymentLinkUseCase,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<CreatePaymentFromProposalUseCase> logger) : ICreatePaymentFromProposalUseCase
{
    public async Task<Result<PaymentResponse?>> ExecuteAsync(Guid proposalId, CreatePaymentFromProposalRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<PaymentResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.PaymentsWrite, cancellationToken))
            return Result<PaymentResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<PaymentResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var proposal = await proposalRepository.GetByIdAndTenantAsync(proposalId, tenantId, cancellationToken);
        if (proposal is null)
            return Result<PaymentResponse?>.Failure("Proposta não encontrada.");
        if (proposal.Status != ProposalStatus.Approved)
            return Result<PaymentResponse?>.Failure("Somente propostas aprovadas podem gerar pagamento.");

        var providerId = await ResolveProviderIdAsync(tenantId, request.TenantPaymentProviderId, cancellationToken);
        if (!providerId.HasValue)
            return Result<PaymentResponse?>.Failure("Informe um provedor de pagamento ou configure um provedor padrão.");

        try
        {
            var payment = new Payment(
                tenantId,
                proposal.QuoteId,
                proposal.Id,
                providerId,
                null,
                request.PaymentMethod,
                PaymentStatus.Pending,
                request.Installments,
                request.GrossAmount,
                request.FeeAmount,
                request.NetAmount,
                request.DueDate,
                createdBy);

            await paymentRepository.AddAsync(payment, cancellationToken);

            if (request.CreatePaymentLink)
            {
                var linkResult = await createPaymentLinkUseCase.ExecuteAsync(
                    payment.Id,
                    new CreatePaymentLinkRequest(null),
                    cancellationToken);

                if (!linkResult.Success)
                    return Result<PaymentResponse?>.Failure(linkResult.Error ?? "Erro ao criar link de pagamento.");
            }

            var fresh = await paymentRepository.GetByIdAndTenantAsync(payment.Id, tenantId, cancellationToken);
            return Result<PaymentResponse?>.Ok(mapper.Map<PaymentResponse>(fresh));
        }
        catch (Exception ex)
        {
            logger.LogError("CreatePaymentFromProposalUseCase | Erro | {Message}", ex.Message);
            return Result<PaymentResponse?>.Failure("Erro inesperado ao criar pagamento.");
        }
    }

    private async Task<Guid?> ResolveProviderIdAsync(Guid tenantId, Guid? requestedProviderId, CancellationToken cancellationToken)
    {
        if (requestedProviderId.HasValue)
        {
            var provider = await tenantPaymentProviderRepository.GetByIdAndTenantAsync(requestedProviderId.Value, tenantId, cancellationToken);
            if (provider is null || provider.Status != PaymentProviderStatus.Active)
                return null;
            return provider.Id;
        }

        var defaultProvider = await tenantPaymentProviderRepository.GetDefaultByTenantAsync(tenantId, cancellationToken);
        if (defaultProvider is null || defaultProvider.Status != PaymentProviderStatus.Active)
            return null;

        return defaultProvider.Id;
    }
}
