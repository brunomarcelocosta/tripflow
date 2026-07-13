using FluentValidation;
using Tripflow.Application.DTOs.Requests.Quotes;

namespace Tripflow.Application.Validators.Quotes;

public sealed class UpdateItineraryRequestValidator : AbstractValidator<UpdateItineraryRequest>
{
    public UpdateItineraryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome do roteiro é obrigatório.")
            .MaximumLength(200);
        RuleFor(x => x.TotalDays).GreaterThanOrEqualTo(0)
            .When(x => x.TotalDays.HasValue);
        RuleFor(x => x.TotalNights).GreaterThanOrEqualTo(0)
            .When(x => x.TotalNights.HasValue);
    }
}
