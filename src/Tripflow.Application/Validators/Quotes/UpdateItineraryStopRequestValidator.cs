using FluentValidation;
using Tripflow.Application.DTOs.Requests.Quotes;

namespace Tripflow.Application.Validators.Quotes;

public sealed class UpdateItineraryStopRequestValidator : AbstractValidator<UpdateItineraryStopRequest>
{
    public UpdateItineraryStopRequestValidator()
    {
        RuleFor(x => x.Country).NotEmpty().WithMessage("O país é obrigatório.").MaximumLength(100);
        RuleFor(x => x.City).NotEmpty().WithMessage("A cidade é obrigatória.").MaximumLength(100);
        RuleFor(x => x.Nights).GreaterThanOrEqualTo(0).WithMessage("Noites não pode ser negativo.");
        RuleFor(x => x.Sequence).GreaterThanOrEqualTo(0).WithMessage("Sequência inválida.");
        RuleFor(x => x.Notes).MaximumLength(4000).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
