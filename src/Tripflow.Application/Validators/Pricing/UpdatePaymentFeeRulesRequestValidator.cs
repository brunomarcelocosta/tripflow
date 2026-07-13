using FluentValidation;
using Tripflow.Application.DTOs.Requests.Pricing;

namespace Tripflow.Application.Validators.Pricing;

public sealed class UpdatePaymentFeeRulesRequestValidator : AbstractValidator<UpdatePaymentFeeRulesRequest>
{
    public UpdatePaymentFeeRulesRequestValidator(IValidator<UpdatePaymentFeeRuleItem> itemValidator)
    {
        RuleFor(x => x.Rules)
            .NotNull()
            .WithMessage("A lista de regras de taxa é obrigatória.")
            .Must(rules => rules.Any())
            .WithMessage("Informe pelo menos uma regra de taxa.");

        RuleFor(x => x.Rules)
            .Must(NotHaveDuplicates)
            .WithMessage("Não é permitido enviar regras duplicadas para o mesmo método e parcelamento.")
            .When(x => x.Rules.Any());

        RuleForEach(x => x.Rules).SetValidator(itemValidator);
    }

    private static bool NotHaveDuplicates(IEnumerable<UpdatePaymentFeeRuleItem> rules)
    {
        var duplicates = rules
            .GroupBy(r => (r.PaymentMethod, r.Installments))
            .Any(g => g.Count() > 1);

        return !duplicates;
    }
}
