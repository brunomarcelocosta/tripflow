using System.Text.RegularExpressions;
using FluentValidation;
using Tripflow.Application.DTOs.Requests.Tenants;

namespace Tripflow.Application.Validators.Tenants;

public sealed partial class UpdateTenantBrandingRequestValidator : AbstractValidator<UpdateTenantBrandingRequest>
{
    public UpdateTenantBrandingRequestValidator()
    {
        RuleFor(x => x.PrimaryColor)
            .Must(BeValidHexColor!)
            .WithMessage("A cor primária deve estar no formato HEX (ex.: #FFFFFF).")
            .When(x => !string.IsNullOrWhiteSpace(x.PrimaryColor));

        RuleFor(x => x.SecondaryColor)
            .Must(BeValidHexColor!)
            .WithMessage("A cor secundária deve estar no formato HEX (ex.: #FFFFFF).")
            .When(x => !string.IsNullOrWhiteSpace(x.SecondaryColor));

        RuleFor(x => x.TextColor)
            .Must(BeValidHexColor!)
            .WithMessage("A cor do texto deve estar no formato HEX (ex.: #FFFFFF).")
            .When(x => !string.IsNullOrWhiteSpace(x.TextColor));

        RuleFor(x => x.ProposalFooter)
            .MaximumLength(4000)
            .WithMessage("O rodapé da proposta deve ter no máximo 4000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.ProposalFooter));
    }

    private static bool BeValidHexColor(string value) => HexColorRegex().IsMatch(value);

    [GeneratedRegex(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", RegexOptions.Compiled)]
    private static partial Regex HexColorRegex();
}
