using FluentValidation;
using Tripflow.Application.DTOs.Requests.Miles;

namespace Tripflow.Application.Validators.Miles;

public sealed class CreateMilesQuoteOptionRequestValidator : AbstractValidator<CreateMilesQuoteOptionRequest>
{
    public CreateMilesQuoteOptionRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.").MaximumLength(100);
        RuleFor(x => x.MilesAmount).GreaterThan(0).WithMessage("Quantidade de milhas deve ser maior que zero.");
        RuleFor(x => x.BoardingFees).GreaterThanOrEqualTo(0).When(x => x.BoardingFees.HasValue);
        RuleFor(x => x.CostPerThousand).GreaterThanOrEqualTo(0).When(x => x.CostPerThousand.HasValue);
        RuleFor(x => x.CashPrice).GreaterThanOrEqualTo(0).When(x => x.CashPrice.HasValue);
        RuleFor(x => x.ServiceFee).GreaterThanOrEqualTo(0).When(x => x.ServiceFee.HasValue);
        RuleFor(x => x.Notes).MaximumLength(4000).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
