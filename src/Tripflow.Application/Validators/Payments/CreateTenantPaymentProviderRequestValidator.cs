using FluentValidation;
using Tripflow.Application.DTOs.Requests.Payments;

namespace Tripflow.Application.Validators.Payments;

public sealed class CreateTenantPaymentProviderRequestValidator : AbstractValidator<CreateTenantPaymentProviderRequest>
{
    public CreateTenantPaymentProviderRequestValidator()
    {
        RuleFor(x => x.PaymentProviderId)
            .NotEmpty()
            .WithMessage("O provedor de pagamento é obrigatório.");

        RuleFor(x => x.DisplayName)
            .MaximumLength(200)
            .WithMessage("O nome de exibição deve ter no máximo 200 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        RuleFor(x => x.ApiKey)
            .MaximumLength(500)
            .WithMessage("A chave de API deve ter no máximo 500 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.ApiKey));

        RuleFor(x => x.Secret)
            .MaximumLength(500)
            .WithMessage("O secret deve ter no máximo 500 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Secret));

        RuleFor(x => x.WebhookSecret)
            .MaximumLength(500)
            .WithMessage("O webhook secret deve ter no máximo 500 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.WebhookSecret));
    }
}
