using FluentValidation;
using Tripflow.Application.DTOs.Requests.Subscriptions;

namespace Tripflow.Application.Validators.Subscriptions;

public sealed class CreatePlatformCheckoutRequestValidator : AbstractValidator<CreatePlatformCheckoutRequest>
{
    public CreatePlatformCheckoutRequestValidator()
    {
        RuleFor(x => x.PlanId)
            .NotEmpty().WithMessage("O plano é obrigatório.");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("O nome da empresa é obrigatório.")
            .MaximumLength(200);

        RuleFor(x => x.ResponsibleName)
            .NotEmpty().WithMessage("O nome do responsável é obrigatório.")
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O e-mail informado é inválido.");

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.BillingCycle)
            .Must(c => c is "monthly" or "annual")
            .WithMessage("O ciclo de cobrança deve ser monthly ou annual.");
    }
}
