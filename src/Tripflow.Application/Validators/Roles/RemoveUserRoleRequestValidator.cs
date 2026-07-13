using FluentValidation;
using Tripflow.Application.DTOs.Requests.Roles;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.Application.Validators.Roles;

public sealed class RemoveUserRoleRequestValidator : AbstractValidator<RemoveUserRoleValidationRequest>
{
    public RemoveUserRoleRequestValidator(
        ITenantContext tenantContext,
        IUserProfileRepository userProfileRepository,
        IRoleRepository roleRepository)
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("O identificador do usuário é obrigatório.");

        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("O identificador da role é obrigatório.");

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

        RuleFor(x => x.RoleId)
            .MustAsync(async (roleId, cancellationToken) =>
            {
                if (!tenantContext.HasTenant || tenantContext.TenantId is null)
                    return false;

                var role = await roleRepository.GetByIdAsync(roleId);

                return role is not null && role.TenantId == tenantContext.TenantId.Value;
            })
            .WithMessage("Role não encontrada nesta empresa.");
    }
}
