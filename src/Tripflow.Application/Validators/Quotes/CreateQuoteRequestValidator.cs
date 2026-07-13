using FluentValidation;
using Tripflow.Application.DTOs.Requests.Quotes;

namespace Tripflow.Application.Validators.Quotes;

public sealed class CreateQuoteRequestValidator : AbstractValidator<CreateQuoteRequest>
{
    public CreateQuoteRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título da cotação é obrigatório.")
            .MaximumLength(200).WithMessage("O título deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Type).IsInEnum().WithMessage("O tipo da cotação é inválido.");

        RuleFor(x => x.Origin).MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Origin));
        RuleFor(x => x.Destination).MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Destination));

        RuleFor(x => x.Adults).GreaterThanOrEqualTo(1).WithMessage("É necessário ao menos 1 adulto.");
        RuleFor(x => x.Children).GreaterThanOrEqualTo(0).WithMessage("Crianças não pode ser negativo.");
        RuleFor(x => x.Infants).GreaterThanOrEqualTo(0).WithMessage("Bebês não pode ser negativo.");

        RuleFor(x => x.Notes).MaximumLength(8000)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));

        RuleFor(x => x)
            .Must(x => !(x.DepartureDate.HasValue && x.ReturnDate.HasValue && x.ReturnDate.Value < x.DepartureDate.Value))
            .WithMessage("A data de volta não pode ser anterior à data de ida.");
    }
}
