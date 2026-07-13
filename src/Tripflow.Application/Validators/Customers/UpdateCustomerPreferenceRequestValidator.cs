using FluentValidation;
using Tripflow.Application.DTOs.Requests.Customers;

namespace Tripflow.Application.Validators.Customers;

public sealed class UpdateCustomerPreferenceRequestValidator : AbstractValidator<UpdateCustomerPreferenceRequest>
{
    public UpdateCustomerPreferenceRequestValidator()
    {
        RuleFor(x => x.PreferredAirlines)
            .MaximumLength(1000)
            .WithMessage("As companhias preferidas devem ter no máximo 1000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.PreferredAirlines));

        RuleFor(x => x.PreferredHotelCategories)
            .MaximumLength(1000)
            .WithMessage("As categorias de hotel preferidas devem ter no máximo 1000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.PreferredHotelCategories));

        RuleFor(x => x.SeatPreferences)
            .MaximumLength(1000)
            .WithMessage("As preferências de assento devem ter no máximo 1000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.SeatPreferences));

        RuleFor(x => x.MealRestrictions)
            .MaximumLength(1000)
            .WithMessage("As restrições alimentares devem ter no máximo 1000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.MealRestrictions));

        RuleFor(x => x.TravelPreferences)
            .MaximumLength(2000)
            .WithMessage("As preferências de viagem devem ter no máximo 2000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.TravelPreferences));

        RuleFor(x => x.GeneralNotes)
            .MaximumLength(4000)
            .WithMessage("As observações gerais devem ter no máximo 4000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.GeneralNotes));
    }
}
