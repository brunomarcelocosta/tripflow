using FluentValidation;
using Tripflow.Application.DTOs.Requests.Roles;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.Application.Validators.Roles;

public sealed class AssignUserRolesRequestValidator : AbstractValidator<AssignUserRolesValidationRequest>
{
    public AssignUserRolesRequestValidator(
        ITenantContext tenantContext,
        IUserProfileRepository userProfileRepository)
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("O identificador do usuário é obrigatório.");

        RuleFor(x => x)
            .Must(x => (x.RoleIds?.Count ?? 0) > 0 || (x.RoleNames?.Count ?? 0) > 0)
            .WithMessage("Informe ao menos uma role válida.");

        RuleFor(x => x)
            .Must(x => (x.RoleIds?.Count ?? 0) == 0 || (x.RoleNames?.Count ?? 0) == 0)
            .WithMessage("Informe apenas RoleIds ou RoleNames, não ambos.");

        RuleFor(x => x.UserId)
            .MustAsync(async (userId, cancellationToken) =>
            {
                if (!tenantContext.HasTenant || tenantContext.TenantId is null)
                    return false;

                var profile = await userProfileRepository.GetByIdInTenantAsync(
                    tenantContext.TenantId.Value,
                    userId,
                    cancellationToken);

                return profile is not null;
            })
            .WithMessage("Usuário não encontrado nesta empresa.");
    }
}
