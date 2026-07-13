using FluentValidation;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.Validators.Tenants;

public sealed class UpdateTenantRequestValidator : AbstractValidator<UpdateTenantValidationRequest>
{
    public UpdateTenantRequestValidator(ITenantRepository repository)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do tenant é obrigatório.")
            .MustAsync(async (id, cancellationToken) =>
                await repository.AnyAsync(t => t.Id == id, cancellationToken))
            .WithMessage("Tenant não encontrado.");

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

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("O status informado é inválido.");

        RuleFor(x => x.DocumentNumber)
            .MaximumLength(20)
            .WithMessage("O documento deve ter no máximo 20 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.DocumentNumber));

        RuleFor(x => x)
            .MustAsync(async (request, cancellationToken) =>
                !await repository.AnyAsync(
                    t => t.DocumentNumber == request.DocumentNumber && t.Id != request.Id,
                    cancellationToken))
            .WithMessage("Já existe outro tenant cadastrado com este documento.")
            .When(x => !string.IsNullOrWhiteSpace(x.DocumentNumber));

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("O e-mail informado é inválido.")
            .MaximumLength(256)
            .WithMessage("O e-mail deve ter no máximo 256 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x)
            .MustAsync(async (request, cancellationToken) =>
                !await repository.AnyAsync(
                    t => t.Email == request.Email && t.Id != request.Id,
                    cancellationToken))
            .WithMessage("Já existe outro tenant cadastrado com este e-mail.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .WithMessage("O telefone deve ter no máximo 20 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }
}
