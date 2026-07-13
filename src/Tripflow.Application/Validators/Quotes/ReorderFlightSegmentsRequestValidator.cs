using FluentValidation;
using Tripflow.Application.DTOs.Requests.Quotes;

namespace Tripflow.Application.Validators.Quotes;

public sealed class ReorderFlightSegmentsRequestValidator : AbstractValidator<ReorderFlightSegmentsRequest>
{
    public ReorderFlightSegmentsRequestValidator()
    {
        RuleFor(x => x.Segments).NotEmpty().WithMessage("Informe ao menos um segmento para reordenar.");

        RuleForEach(x => x.Segments).ChildRules(seg =>
        {
            seg.RuleFor(s => s.SegmentId).NotEmpty().WithMessage("SegmentId é obrigatório.");
            seg.RuleFor(s => s.Sequence).GreaterThanOrEqualTo(0).WithMessage("Sequência inválida.");
        });
    }
}
