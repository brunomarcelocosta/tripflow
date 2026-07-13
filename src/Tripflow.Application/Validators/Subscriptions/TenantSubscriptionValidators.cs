using FluentValidation;
using Tripflow.Application.DTOs.Requests.Subscriptions;

namespace Tripflow.Application.Validators.Subscriptions;

public sealed class UpdateTenantSubscriptionRequestValidator : AbstractValidator<UpdateTenantSubscriptionRequest>
{
    public UpdateTenantSubscriptionRequestValidator()
    {
        RuleFor(x => x.SubscriptionPlanId)
            .NotEmpty()
            .WithMessage("O plano da assinatura é obrigatório.");

        RuleFor(x => x.StartedAtUtc)
            .NotEmpty()
            .WithMessage("A data de início é obrigatória.");
    }
}
