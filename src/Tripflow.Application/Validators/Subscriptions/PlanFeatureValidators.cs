using FluentValidation;
using Tripflow.Application.DTOs.Requests.Subscriptions;

namespace Tripflow.Application.Validators.Subscriptions;

public sealed class UpdatePlanFeatureItemValidator : AbstractValidator<UpdatePlanFeatureItem>
{
    public UpdatePlanFeatureItemValidator()
    {
        RuleFor(x => x.FeatureCode)
            .NotEmpty()
            .WithMessage("O código da feature é obrigatório.")
            .MaximumLength(100)
            .WithMessage("O código da feature deve ter no máximo 100 caracteres.");

        RuleFor(x => x.LimitValue)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O limite da feature deve ser maior ou igual a zero.")
            .When(x => x.LimitValue.HasValue);
    }
}

public sealed class UpdatePlanFeaturesRequestValidator : AbstractValidator<UpdatePlanFeaturesRequest>
{
    public UpdatePlanFeaturesRequestValidator(IValidator<UpdatePlanFeatureItem> itemValidator)
    {
        RuleFor(x => x.Features)
            .NotNull()
            .WithMessage("A lista de features é obrigatória.")
            .Must(features => features.Any())
            .WithMessage("Informe pelo menos uma feature.");

        RuleFor(x => x.Features)
            .Must(features =>
                !features.GroupBy(f => f.FeatureCode.Trim().ToLowerInvariant()).Any(g => g.Count() > 1))
            .WithMessage("Não é permitido enviar features duplicadas para o mesmo código.")
            .When(x => x.Features.Any());

        RuleForEach(x => x.Features).SetValidator(itemValidator);
    }
}
