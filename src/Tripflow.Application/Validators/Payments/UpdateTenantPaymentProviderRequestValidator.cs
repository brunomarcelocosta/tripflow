using FluentValidation;
using Tripflow.Application.DTOs.Requests.Payments;

namespace Tripflow.Application.Validators.Payments;

public sealed class UpdateTenantPaymentProviderRequestValidator : AbstractValidator<UpdateTenantPaymentProviderRequest>
{
    public UpdateTenantPaymentProviderRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("O status do provedor é inválido.");

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
