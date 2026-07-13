using FluentValidation;
using Tripflow.Application.DTOs.Requests.Customers;

namespace Tripflow.Application.Validators.Customers;

public sealed class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("O tipo de cliente é inválido.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("O nome completo é obrigatório.")
            .MaximumLength(200)
            .WithMessage("O nome completo deve ter no máximo 200 caracteres.");

        RuleFor(x => x.DocumentNumber)
            .MaximumLength(30)
            .WithMessage("O documento deve ter no máximo 30 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.DocumentNumber));

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("O e-mail informado é inválido.")
            .MaximumLength(200)
            .WithMessage("O e-mail deve ter no máximo 200 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(50)
            .WithMessage("O telefone deve ter no máximo 50 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

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
