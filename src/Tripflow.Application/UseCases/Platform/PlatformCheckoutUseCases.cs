using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.DTOs.Responses.Subscriptions;
using Tripflow.Application.Options;
using Tripflow.Application.Services.Payments.InfinitePay;
using Tripflow.Application.UseCases.Platform.Interfaces;
using Tripflow.Domain.Constants;
using Tripflow.Domain.Entities.Platform;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.Application.UseCases.Platform;

internal static class PublicSubscriptionMapping
{
    public static PublicSubscriptionPlanResponse Map(SubscriptionPlan plan)
    {
        var enabledFeatures = (plan.Features ?? [])
            .Where(x => x.Enabled)
            .Select(x => new PublicPlanFeatureResponse
            {
                Code = x.FeatureCode,
                Label = PlanFeatureCodes.GetLabel(x.FeatureCode),
                LimitValue = x.LimitValue,
                Enabled = x.Enabled
            })
            .OrderBy(x => x.Label)
            .ToList();

        return new PublicSubscriptionPlanResponse
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            MonthlyPrice = plan.MonthlyPrice,
            AnnualPrice = plan.AnnualPrice,
            MaxUsers = plan.MaxUsers,
            MaxQuotesPerMonth = plan.MaxQuotesPerMonth,
            IsActive = plan.Status == SubscriptionPlanStatus.Active,
            RequiresContact = !plan.MonthlyPrice.HasValue && !plan.AnnualPrice.HasValue,
            Features = enabledFeatures
        };
    }

    public static TenantEntitlementsResponse MapEntitlements(TenantSubscription? subscription)
    {
        if (subscription?.SubscriptionPlan is null)
        {
            return new TenantEntitlementsResponse
            {
                Features = []
            };
        }

        var plan = subscription.SubscriptionPlan;

        return new TenantEntitlementsResponse
        {
            SubscriptionPlanId = plan.Id,
            SubscriptionPlanName = plan.Name,
            SubscriptionStatus = subscription.Status.ToString(),
            MaxUsers = plan.MaxUsers,
            MaxQuotesPerMonth = plan.MaxQuotesPerMonth,
            Features = (plan.Features ?? [])
                .Where(x => x.Enabled)
                .Select(x => new PublicPlanFeatureResponse
                {
                    Code = x.FeatureCode,
                    Label = PlanFeatureCodes.GetLabel(x.FeatureCode),
                    LimitValue = x.LimitValue,
                    Enabled = x.Enabled
                })
                .OrderBy(x => x.Label)
                .ToList()
        };
    }
}

public sealed class GetPublicSubscriptionPlansUseCase(
    ISubscriptionPlanRepository subscriptionPlanRepository) : IGetPublicSubscriptionPlansUseCase
{
    public async Task<Result<IReadOnlyList<PublicSubscriptionPlanResponse>>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var plans = await subscriptionPlanRepository.GetActiveWithFeaturesAsync(cancellationToken);

        var mapped = plans
            .OrderBy(x => x.MonthlyPrice ?? decimal.MaxValue)
            .Select(PublicSubscriptionMapping.Map)
            .ToList();

        return Result<IReadOnlyList<PublicSubscriptionPlanResponse>>.Ok(mapped);
    }
}

public sealed class CreatePlatformCheckoutUseCase(
    ISubscriptionPlanRepository subscriptionPlanRepository,
    ILeadRepository leadRepository,
    IPlatformCheckoutSessionRepository checkoutSessionRepository,
    IInfinitePayCheckoutService infinitePayCheckoutService,
    IValidator<CreatePlatformCheckoutRequest> validator,
    IOptions<InfinitePayOptions> infinitePayOptions,
    IOptions<FrontendOptions> frontendOptions,
    ILogger<CreatePlatformCheckoutUseCase> logger) : ICreatePlatformCheckoutUseCase
{
    public async Task<Result<CreatePlatformCheckoutResponse?>> ExecuteAsync(
        CreatePlatformCheckoutRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<CreatePlatformCheckoutResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var plan = await subscriptionPlanRepository.GetByIdWithFeaturesAsync(request.PlanId, cancellationToken);
        if (plan is null || plan.Status != SubscriptionPlanStatus.Active)
            return Result<CreatePlatformCheckoutResponse?>.Failure("Plano não encontrado ou indisponível.");

        var isAnnual = string.Equals(request.BillingCycle, "annual", StringComparison.OrdinalIgnoreCase);
        var amount = isAnnual ? plan.AnnualPrice : plan.MonthlyPrice;

        if (!amount.HasValue || amount.Value <= 0)
            return Result<CreatePlatformCheckoutResponse?>.Failure("Este plano requer contato comercial. Entre em contato conosco.");

        await leadRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var lead = new Lead(
                request.CompanyName.Trim(),
                request.ResponsibleName.Trim(),
                normalizedEmail,
                request.Phone?.Trim(),
                plan.Name,
                message: null,
                createdBy: "public",
                source: "platform_checkout");

            lead.AssignSubscriptionPlan(plan.Id, "public");

            await leadRepository.AddAsync(lead, cancellationToken);

            var session = new PlatformCheckoutSession(
                lead.Id,
                plan.Id,
                amount.Value,
                infinitePayOptions.Value.PlatformDefaultCurrency,
                PlatformCheckoutStatus.Pending,
                DateTime.UtcNow.AddHours(24),
                "public");

            await checkoutSessionRepository.AddAsync(session, cancellationToken);

            lead.AssignCheckoutSession(session.Id, "public");
            await leadRepository.UpdateAsync(lead, cancellationToken);

            var frontendBase = (frontendOptions.Value.PublicSiteBaseUrl ?? "http://localhost:5173").TrimEnd('/');
            var successUrl = $"{frontendBase}/checkout/success?sessionId={session.Id}";
            var cancelUrl = $"{frontendBase}/checkout/cancel?sessionId={session.Id}";

            var checkout = await infinitePayCheckoutService.CreatePlatformCheckoutAsync(
                new CreateInfinitePayPlatformCheckoutCommand(
                    request.ResponsibleName.Trim(),
                    normalizedEmail,
                    request.Phone?.Trim(),
                    plan.Name,
                    plan.Id,
                    amount.Value,
                    infinitePayOptions.Value.PlatformDefaultCurrency,
                    successUrl,
                    cancelUrl,
                    session.Id.ToString("N")),
                cancellationToken);

            session.SetProviderDetails(checkout.ExternalCheckoutId, checkout.CheckoutUrl, checkout.RawResponse, "public");
            await checkoutSessionRepository.UpdateAsync(session, cancellationToken);

            await leadRepository.CommitTransactionAsync(cancellationToken);

            return Result<CreatePlatformCheckoutResponse?>.Ok(new CreatePlatformCheckoutResponse
            {
                LeadId = lead.Id,
                CheckoutSessionId = session.Id,
                CheckoutUrl = checkout.CheckoutUrl,
                Amount = amount.Value,
                Currency = infinitePayOptions.Value.PlatformDefaultCurrency,
                Status = PlatformCheckoutStatus.Active.ToString()
            });
        }
        catch (Exception ex)
        {
            await leadRepository.RollbackTransactionAsync(cancellationToken);
            logger.LogError(ex, "CreatePlatformCheckoutUseCase | Erro ao criar checkout.");
            return Result<CreatePlatformCheckoutResponse?>.Failure("Não foi possível iniciar o pagamento. Tente novamente.");
        }
    }
}

public sealed class GetPlatformCheckoutStatusUseCase(
    IPlatformCheckoutSessionRepository checkoutSessionRepository,
    ISubscriptionPlanRepository subscriptionPlanRepository) : IGetPlatformCheckoutStatusUseCase
{
    public async Task<Result<PlatformCheckoutStatusResponse?>> ExecuteAsync(
        Guid checkoutSessionId,
        CancellationToken cancellationToken = default)
    {
        var session = await checkoutSessionRepository.GetByIdAsync(checkoutSessionId, cancellationToken);
        if (session is null)
            return Result<PlatformCheckoutStatusResponse?>.Ok(null);

        var plan = await subscriptionPlanRepository.GetByIdWithFeaturesAsync(session.SubscriptionPlanId, cancellationToken);

        return Result<PlatformCheckoutStatusResponse?>.Ok(new PlatformCheckoutStatusResponse
        {
            CheckoutSessionId = session.Id,
            Status = session.Status.ToString(),
            Amount = session.Amount,
            Currency = session.Currency,
            PlanName = plan?.Name ?? string.Empty,
            PaidAtUtc = session.PaidAtUtc
        });
    }
}

public sealed class ProcessPlatformCheckoutWebhookUseCase(
    IInfinitePayWebhookService infinitePayWebhookService,
    IPlatformCheckoutSessionRepository checkoutSessionRepository,
    IPlatformPaymentEventRepository paymentEventRepository,
    ILeadRepository leadRepository,
    IOptions<InfinitePayOptions> infinitePayOptions,
    ILogger<ProcessPlatformCheckoutWebhookUseCase> logger) : IProcessPlatformCheckoutWebhookUseCase
{
    public async Task<Result<bool>> ExecuteAsync(
        string payload,
        IReadOnlyDictionary<string, string> headers,
        CancellationToken cancellationToken = default)
    {
        var isValid = await infinitePayWebhookService.ValidateSignatureAsync(
            payload,
            headers,
            infinitePayOptions.Value.WebhookSecret,
            cancellationToken);

        if (!isValid)
            return Result<bool>.Failure("Assinatura do webhook inválida.");

        var parsed = await infinitePayWebhookService.ParseAsync(payload, headers, cancellationToken);
        if (parsed is null)
            return Result<bool>.Failure("Payload do webhook inválido.");

        if (await paymentEventRepository.ExistsByProviderAndExternalEventIdAsync(
                parsed.ProviderCode,
                parsed.ExternalEventId,
                cancellationToken))
            return Result<bool>.Ok(true);

        PlatformCheckoutSession? session = null;

        if (!string.IsNullOrWhiteSpace(parsed.ExternalCheckoutId))
            session = await checkoutSessionRepository.GetTrackedByExternalCheckoutIdAsync(parsed.ExternalCheckoutId, cancellationToken);

        if (session is null && Guid.TryParse(parsed.ExternalPaymentId, out var sessionId))
            session = await checkoutSessionRepository.GetTrackedByIdAsync(sessionId, cancellationToken);

        await paymentEventRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var paymentEvent = new PlatformPaymentEvent(
                parsed.ProviderCode,
                parsed.ExternalEventId,
                parsed.ExternalCheckoutId,
                payload);

            await paymentEventRepository.AddAsync(paymentEvent, cancellationToken);

            if (session is not null && session.Status != PlatformCheckoutStatus.Paid)
            {
                session.MarkAsPaid(parsed.PaidAtUtc, "webhook");
                await checkoutSessionRepository.UpdateAsync(session, cancellationToken);

                var lead = await leadRepository.GetTrackedByIdAsync(session.LeadId, cancellationToken);
                if (lead is not null)
                {
                    lead.MarkAsPaid(parsed.PaidAtUtc, "webhook");
                    await leadRepository.UpdateAsync(lead, cancellationToken);
                }
            }

            paymentEvent.MarkAsProcessed();
            await paymentEventRepository.UpdateAsync(paymentEvent, cancellationToken);

            await paymentEventRepository.CommitTransactionAsync(cancellationToken);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            await paymentEventRepository.RollbackTransactionAsync(cancellationToken);
            logger.LogError(ex, "ProcessPlatformCheckoutWebhookUseCase | Erro ao processar webhook.");
            return Result<bool>.Failure("Erro ao processar webhook de pagamento.");
        }
    }
}

public sealed class GetTenantEntitlementsUseCase(
    ITenantSubscriptionRepository tenantSubscriptionRepository,
    IUserContext userContext,
    ITenantContext tenantContext) : IGetTenantEntitlementsUseCase
{
    public async Task<Result<TenantEntitlementsResponse?>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<TenantEntitlementsResponse?>.Forbidden();

        var subscription = await tenantSubscriptionRepository.GetByTenantIdWithPlanFeaturesAsync(
            tenantContext.TenantId.Value,
            cancellationToken);
        return Result<TenantEntitlementsResponse?>.Ok(PublicSubscriptionMapping.MapEntitlements(subscription));
    }
}
