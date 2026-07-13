using FluentValidation;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.Validators.Admin;

public sealed class UpdateAdminUserRequestValidator : AbstractValidator<UpdateAdminUserRequest>
{
    public UpdateAdminUserRequestValidator(IUserProfileRepository repository)
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("O nome completo é obrigatório.")
            .MaximumLength(200).WithMessage("O nome completo deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O e-mail informado é inválido.")
            .MaximumLength(256).WithMessage("O e-mail deve ter no máximo 256 caracteres.");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("O telefone deve ter no máximo 20 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("O status informado é inválido.");
    }
}

public sealed class SetAdminUserPasswordRequestValidator : AbstractValidator<SetAdminUserPasswordRequest>
{
    public SetAdminUserPasswordRequestValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter no mínimo 8 caracteres.")
            .MaximumLength(128).WithMessage("A senha deve ter no máximo 128 caracteres.");
    }
}
