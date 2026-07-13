using FluentValidation;
using Tripflow.Application.DTOs.Requests.Pricing;

namespace Tripflow.Application.Validators.Pricing;

public sealed class UpdateQuotePricingOptionRequestValidator : AbstractValidator<UpdateQuotePricingOptionRequest>
{
    public UpdateQuotePricingOptionRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.").MaximumLength(100);
        RuleFor(x => x.AgencyCost).GreaterThanOrEqualTo(0).WithMessage("Custo da agência não pode ser negativo.");
        RuleFor(x => x.DesiredProfitAmount).GreaterThanOrEqualTo(0).When(x => x.DesiredProfitAmount.HasValue);
        RuleFor(x => x.DesiredProfitPercentage).GreaterThanOrEqualTo(0).When(x => x.DesiredProfitPercentage.HasValue);
        RuleFor(x => x.PixDiscountPercentage)
            .InclusiveBetween(0, 100).When(x => x.PixDiscountPercentage.HasValue)
            .WithMessage("Desconto Pix deve estar entre 0 e 100.");
        RuleFor(x => x.InternalNotes).MaximumLength(4000).When(x => !string.IsNullOrWhiteSpace(x.InternalNotes));

        RuleFor(x => x)
            .Must(x => !(x.DesiredProfitAmount.HasValue && x.DesiredProfitPercentage.HasValue))
            .WithMessage("Informe apenas valor OU percentual de lucro, não ambos.");
    }
}
