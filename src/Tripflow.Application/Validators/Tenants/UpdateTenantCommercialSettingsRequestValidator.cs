using FluentValidation;
using Tripflow.Application.DTOs.Requests.Tenants;

namespace Tripflow.Application.Validators.Tenants;

public sealed class UpdateTenantCommercialSettingsRequestValidator : AbstractValidator<UpdateTenantCommercialSettingsRequest>
{
    public UpdateTenantCommercialSettingsRequestValidator()
    {
        RuleFor(x => x.CommercialEmail)
            .EmailAddress()
            .WithMessage("O e-mail comercial informado é inválido.")
            .MaximumLength(200)
            .WithMessage("O e-mail comercial deve ter no máximo 200 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.CommercialEmail));

        RuleFor(x => x.CommercialPhone)
            .MaximumLength(50)
            .WithMessage("O telefone comercial deve ter no máximo 50 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.CommercialPhone));

        RuleFor(x => x.WhatsApp)
            .MaximumLength(50)
            .WithMessage("O WhatsApp deve ter no máximo 50 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.WhatsApp));

        RuleFor(x => x.Instagram)
            .MaximumLength(150)
            .WithMessage("O Instagram deve ter no máximo 150 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Instagram));

        RuleFor(x => x.Website)
            .MaximumLength(300)
            .WithMessage("O site deve ter no máximo 300 caracteres.")
            .Must(BeValidUrl!)
            .WithMessage("O site informado é inválido.")
            .When(x => !string.IsNullOrWhiteSpace(x.Website));

        RuleFor(x => x.DefaultTerms)
            .MaximumLength(8000)
            .WithMessage("Os termos padrão devem ter no máximo 8000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.DefaultTerms));

        RuleFor(x => x.DefaultImportantNotes)
            .MaximumLength(8000)
            .WithMessage("As observações importantes devem ter no máximo 8000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.DefaultImportantNotes));

        RuleFor(x => x.DefaultProposalExpirationHours)
            .GreaterThan(0)
            .WithMessage("A validade padrão da proposta deve ser maior que 0 horas.");
    }

    private static bool BeValidUrl(string value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
