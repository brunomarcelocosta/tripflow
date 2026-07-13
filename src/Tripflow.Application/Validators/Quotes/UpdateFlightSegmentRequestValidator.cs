using FluentValidation;
using Tripflow.Application.DTOs.Requests.Quotes;

namespace Tripflow.Application.Validators.Quotes;

public sealed class UpdateFlightSegmentRequestValidator : AbstractValidator<UpdateFlightSegmentRequest>
{
    public UpdateFlightSegmentRequestValidator()
    {
        RuleFor(x => x.OriginAirport).NotEmpty().WithMessage("Aeroporto de origem é obrigatório.")
            .MaximumLength(10);
        RuleFor(x => x.DestinationAirport).NotEmpty().WithMessage("Aeroporto de destino é obrigatório.")
            .MaximumLength(10);
        RuleFor(x => x.OriginCity).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.OriginCity));
        RuleFor(x => x.DestinationCity).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.DestinationCity));
        RuleFor(x => x.FlightNumber).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.FlightNumber));
        RuleFor(x => x.AirlineName).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.AirlineName));
        RuleFor(x => x.Sequence).GreaterThanOrEqualTo(0).WithMessage("Sequência inválida.");

        RuleFor(x => x)
            .Must(x => !(x.DepartureDateTimeUtc.HasValue && x.ArrivalDateTimeUtc.HasValue
                && x.ArrivalDateTimeUtc.Value < x.DepartureDateTimeUtc.Value))
            .WithMessage("Horário de chegada não pode ser anterior ao de partida.");
    }
}
