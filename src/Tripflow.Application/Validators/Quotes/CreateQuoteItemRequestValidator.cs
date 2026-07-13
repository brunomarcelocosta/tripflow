using FluentValidation;
using Tripflow.Application.DTOs.Requests.Quotes;

namespace Tripflow.Application.Validators.Quotes;

public sealed class CreateQuoteItemRequestValidator : AbstractValidator<CreateQuoteItemRequest>
{
    public CreateQuoteItemRequestValidator()
    {
        RuleFor(x => x.Type).IsInEnum().WithMessage("O tipo do item é inválido.");
        RuleFor(x => x.Title).NotEmpty().WithMessage("O título do item é obrigatório.")
            .MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
        RuleFor(x => x.AgencyCost).GreaterThanOrEqualTo(0).WithMessage("Custo da agência não pode ser negativo.");
        RuleFor(x => x.SaleAmount).GreaterThanOrEqualTo(0).WithMessage("Valor de venda não pode ser negativo.");
        RuleFor(x => x.Notes).MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
