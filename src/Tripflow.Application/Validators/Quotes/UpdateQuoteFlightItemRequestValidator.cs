using FluentValidation;
using Tripflow.Application.DTOs.Requests.Quotes;

namespace Tripflow.Application.Validators.Quotes;

public sealed class UpdateQuoteFlightItemRequestValidator : AbstractValidator<UpdateQuoteFlightItemRequest>
{
    public UpdateQuoteFlightItemRequestValidator()
    {
        RuleFor(x => x.AirlineName).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.AirlineName));
        RuleFor(x => x.FareFamily).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.FareFamily));
        RuleFor(x => x.BaggageDescription).MaximumLength(4000).When(x => !string.IsNullOrWhiteSpace(x.BaggageDescription));
        RuleFor(x => x.CarryOnWeightKg).GreaterThanOrEqualTo(0).When(x => x.CarryOnWeightKg.HasValue);
        RuleFor(x => x.CheckedBagWeightKg).GreaterThanOrEqualTo(0).When(x => x.CheckedBagWeightKg.HasValue);
    }
}
