using FluentValidation;
using Tripflow.Application.DTOs.Requests.Quotes;

namespace Tripflow.Application.Validators.Quotes;

public sealed class ReorderItineraryStopsRequestValidator : AbstractValidator<ReorderItineraryStopsRequest>
{
    public ReorderItineraryStopsRequestValidator()
    {
        RuleFor(x => x.Stops)
            .NotEmpty().WithMessage("Informe ao menos uma parada para reordenar.");

        RuleForEach(x => x.Stops).ChildRules(stop =>
        {
            stop.RuleFor(s => s.StopId).NotEmpty().WithMessage("StopId é obrigatório.");
            stop.RuleFor(s => s.Sequence).GreaterThanOrEqualTo(0).WithMessage("Sequência inválida.");
        });
    }
}
