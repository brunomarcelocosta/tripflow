using FluentValidation;
using Tripflow.Application.DTOs.Requests.Travelers;

namespace Tripflow.Application.Validators.Travelers;

public sealed class CreateTravelerRequestValidator : AbstractValidator<CreateTravelerRequest>
{
    public CreateTravelerRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("O nome completo é obrigatório.")
            .MaximumLength(200)
            .WithMessage("O nome completo deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Nationality)
            .MaximumLength(100)
            .WithMessage("A nacionalidade deve ter no máximo 100 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Nationality));

        RuleFor(x => x.DocumentNumber)
            .MaximumLength(50)
            .WithMessage("O documento deve ter no máximo 50 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.DocumentNumber));

        RuleFor(x => x.PassportNumber)
            .MaximumLength(50)
            .WithMessage("O número do passaporte deve ter no máximo 50 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.PassportNumber));

        RuleFor(x => x.Notes)
            .MaximumLength(4000)
            .WithMessage("As observações devem ter no máximo 4000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));

        RuleFor(x => x.BirthDate)
            .Must(BeNotInFuture!)
            .WithMessage("A data de nascimento não pode ser futura.")
            .When(x => x.BirthDate.HasValue);
    }

    private static bool BeNotInFuture(DateOnly? birthDate)
        => !birthDate.HasValue || birthDate.Value <= DateOnly.FromDateTime(DateTime.UtcNow);
}
