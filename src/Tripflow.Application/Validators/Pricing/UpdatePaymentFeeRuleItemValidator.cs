using FluentValidation;
using Tripflow.Application.DTOs.Requests.Pricing;
using Tripflow.Domain.Enums;

namespace Tripflow.Application.Validators.Pricing;

public sealed class UpdatePaymentFeeRuleItemValidator : AbstractValidator<UpdatePaymentFeeRuleItem>
{
    public UpdatePaymentFeeRuleItemValidator()
    {
        RuleFor(x => x.PaymentMethod)
            .IsInEnum()
            .WithMessage("O método de pagamento informado é inválido.");

        RuleFor(x => x.Installments)
            .GreaterThan(0)
            .WithMessage("O número de parcelas é obrigatório e deve ser maior que zero.");

        RuleFor(x => x.Installments)
            .Equal(1)
            .WithMessage("Para PIX e Manual o número de parcelas deve ser 1.")
            .When(x => x.PaymentMethod is PaymentMethod.Pix or PaymentMethod.Manual);

        RuleFor(x => x.Installments)
            .InclusiveBetween(1, 12)
            .WithMessage("Para cartão de crédito o número de parcelas deve estar entre 1 e 12.")
            .When(x => x.PaymentMethod == PaymentMethod.CreditCard);

        RuleFor(x => x.FeePercentage)
            .GreaterThanOrEqualTo(0)
            .WithMessage("A taxa não pode ser negativa.");
    }
}
