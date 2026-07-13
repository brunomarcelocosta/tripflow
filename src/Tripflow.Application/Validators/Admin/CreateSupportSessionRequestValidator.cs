using FluentValidation;
using Tripflow.Application.DTOs.Requests.Admin;

namespace Tripflow.Application.Validators.Admin;

public sealed class CreateSupportSessionRequestValidator : AbstractValidator<CreateSupportSessionRequest>
{
    public CreateSupportSessionRequestValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("O tenant é obrigatório.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("O motivo da sessão de suporte é obrigatório.")
            .MaximumLength(500)
            .WithMessage("O motivo deve ter no máximo 500 caracteres.");
    }
}
