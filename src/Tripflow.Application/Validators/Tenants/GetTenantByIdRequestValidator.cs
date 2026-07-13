using FluentValidation;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.Validators.Tenants;

public sealed class GetTenantByIdRequestValidator : AbstractValidator<GetTenantByIdRequest>
{
    public GetTenantByIdRequestValidator(ITenantRepository repository)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do tenant é obrigatório.")
            .MustAsync(async (id, cancellationToken) =>
                await repository.AnyAsync(t => t.Id == id, cancellationToken))
            .WithMessage("Tenant não encontrado.");
    }
}
