using FluentValidation;
using Tripflow.Application.DTOs.Requests.Payments;
using Tripflow.Domain.Enums;

namespace Tripflow.Application.Validators.Payments;

public sealed class CreatePaymentFromProposalRequestValidator : AbstractValidator<CreatePaymentFromProposalRequest>
{
    public CreatePaymentFromProposalRequestValidator()
    {
        RuleFor(x => x.PaymentMethod)
            .IsInEnum()
            .WithMessage("O método de pagamento é inválido.");

        RuleFor(x => x.GrossAmount)
            .GreaterThan(0)
            .WithMessage("O valor bruto deve ser maior que zero.");

        RuleFor(x => x.FeeAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("A taxa não pode ser negativa.")
            .When(x => x.FeeAmount.HasValue);

        RuleFor(x => x.NetAmount)
            .GreaterThan(0)
            .WithMessage("O valor líquido deve ser maior que zero.")
            .When(x => x.NetAmount.HasValue);

        RuleFor(x => x.Installments)
            .Equal(1)
            .WithMessage("Pix, boleto e pagamento manual devem ter 1 parcela.")
            .When(x => x.PaymentMethod is PaymentMethod.Pix or PaymentMethod.Manual or PaymentMethod.BankSlip);

        RuleFor(x => x.Installments)
            .InclusiveBetween(1, 12)
            .WithMessage("Cartão de crédito deve ter entre 1 e 12 parcelas.")
            .When(x => x.PaymentMethod == PaymentMethod.CreditCard);

        RuleFor(x => x)
            .Must(x => !x.NetAmount.HasValue || !x.FeeAmount.HasValue || x.NetAmount.Value <= x.GrossAmount)
            .WithMessage("O valor líquido não pode ser maior que o valor bruto.");
    }
}
