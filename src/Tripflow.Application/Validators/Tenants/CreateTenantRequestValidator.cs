using FluentValidation;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.Validators.Tenants;

public sealed class CreateTenantRequestValidator : AbstractValidator<CreateTenantRequest>
{
    public CreateTenantRequestValidator(ITenantRepository repository)
    {
        RuleFor(x => x.LegalName)
            .NotEmpty()
            .WithMessage("A razão social é obrigatória.")
            .MaximumLength(200)
            .WithMessage("A razão social deve ter no máximo 200 caracteres.");

        RuleFor(x => x.TradeName)
            .NotEmpty()
            .WithMessage("O nome fantasia é obrigatório.")
            .MaximumLength(200)
            .WithMessage("O nome fantasia deve ter no máximo 200 caracteres.");

        RuleFor(x => x.DocumentNumber)
            .MaximumLength(20)
            .WithMessage("O documento deve ter no máximo 20 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.DocumentNumber));

        RuleFor(x => x.DocumentNumber)
            .MustAsync(async (documentNumber, cancellationToken) =>
                !await repository.AnyAsync(
                    t => t.DocumentNumber == documentNumber,
                    cancellationToken))
            .WithMessage("Já existe um tenant cadastrado com este documento.")
            .When(x => !string.IsNullOrWhiteSpace(x.DocumentNumber));

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("O e-mail informado é inválido.")
            .MaximumLength(256)
            .WithMessage("O e-mail deve ter no máximo 256 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Email)
            .MustAsync(async (email, cancellationToken) =>
                !await repository.AnyAsync(
                    t => t.Email == email,
                    cancellationToken))
            .WithMessage("Já existe um tenant cadastrado com este e-mail.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .WithMessage("O telefone deve ter no máximo 20 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }
}
