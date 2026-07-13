using FluentValidation;
using Tripflow.Application.DTOs.Requests.Users;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.Application.Validators.Users;

public sealed class InviteUserRequestValidator : AbstractValidator<InviteUserRequest>
{
    public InviteUserRequestValidator(
        ITenantContext tenantContext,
        IUserProfileRepository userProfileRepository)
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Nome completo é obrigatório.")
            .MaximumLength(200)
            .WithMessage("O nome completo deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("E-mail é obrigatório.")
            .EmailAddress()
            .WithMessage("O e-mail informado é inválido.")
            .MaximumLength(200)
            .WithMessage("O e-mail deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Phone)
            .MaximumLength(50)
            .WithMessage("O telefone deve ter no máximo 50 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x)
            .Must(x => (x.RoleIds?.Count ?? 0) > 0 || (x.RoleNames?.Count ?? 0) > 0)
            .WithMessage("Informe ao menos uma role válida para o convite.");

        RuleFor(x => x)
            .Must(x => (x.RoleIds?.Count ?? 0) == 0 || (x.RoleNames?.Count ?? 0) == 0)
            .WithMessage("Informe apenas RoleIds ou RoleNames, não ambos.");

        RuleFor(x => x.Email)
            .MustAsync(async (email, cancellationToken) =>
            {
                if (!tenantContext.HasTenant || tenantContext.TenantId is null)
                    return true;

                var normalized = email.Trim().ToLowerInvariant();

                return !await userProfileRepository.ExistsByEmailInTenantAsync(
                    tenantContext.TenantId.Value,
                    normalized,
                    cancellationToken);
            })
            .WithMessage("Já existe um usuário com este e-mail nesta empresa.");
    }
}
