using FluentValidation;
using Tripflow.Application.DTOs.Requests.Tenants;

namespace Tripflow.Application.Validators.Tenants;

public sealed class UpdateTenantFinancialSettingsRequestValidator : AbstractValidator<UpdateTenantFinancialSettingsRequest>
{
    public UpdateTenantFinancialSettingsRequestValidator()
    {
        RuleFor(x => x.DefaultProfitAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("A margem padrão em valor não pode ser negativa.")
            .When(x => x.DefaultProfitAmount.HasValue);

        RuleFor(x => x.DefaultProfitPercentage)
            .GreaterThanOrEqualTo(0)
            .WithMessage("A margem padrão em percentual não pode ser negativa.")
            .When(x => x.DefaultProfitPercentage.HasValue);

        RuleFor(x => x.DefaultPixDiscountPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("O desconto padrão de PIX deve estar entre 0 e 100.")
            .When(x => x.DefaultPixDiscountPercentage.HasValue);

        RuleFor(x => x)
            .Must(x => !(x.DefaultProfitAmount.HasValue && x.DefaultProfitPercentage.HasValue))
            .WithMessage("Informe a margem padrão em valor OU em percentual, não os dois ao mesmo tempo.");
    }
}
