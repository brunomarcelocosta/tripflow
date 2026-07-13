using FluentValidation;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.Validators.Subscriptions;

public sealed class SubscriptionPlanFilterRequestValidator : AbstractValidator<SubscriptionPlanFilterRequest>
{
    public SubscriptionPlanFilterRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("A página deve ser maior que zero.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 200)
            .WithMessage("O tamanho da página deve estar entre 1 e 200.");
    }
}

public sealed class CreateSubscriptionPlanRequestValidator : AbstractValidator<CreateSubscriptionPlanRequest>
{
    public CreateSubscriptionPlanRequestValidator(ISubscriptionPlanRepository repository)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("O nome do plano é obrigatório.")
            .MaximumLength(100)
            .WithMessage("O nome do plano deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Name)
            .MustAsync(async (name, cancellationToken) => !await repository.ExistsByNameAsync(name.Trim(), cancellationToken))
            .WithMessage("Já existe um plano com este nome.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("A descrição deve ter no máximo 1000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.MonthlyPrice)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("O preço mensal deve ser maior ou igual a zero.")
            .When(x => x.MonthlyPrice.HasValue);

        RuleFor(x => x.AnnualPrice)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("O preço anual deve ser maior ou igual a zero.")
            .When(x => x.AnnualPrice.HasValue);

        RuleFor(x => x.MaxUsers)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O limite de usuários deve ser maior ou igual a zero.")
            .When(x => x.MaxUsers.HasValue);

        RuleFor(x => x.MaxQuotesPerMonth)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O limite de cotações deve ser maior ou igual a zero.")
            .When(x => x.MaxQuotesPerMonth.HasValue);
    }
}

public sealed class UpdateSubscriptionPlanRequestValidator : AbstractValidator<UpdateSubscriptionPlanRequest>
{
    public UpdateSubscriptionPlanRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("O nome do plano é obrigatório.")
            .MaximumLength(100)
            .WithMessage("O nome do plano deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("A descrição deve ter no máximo 1000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.MonthlyPrice)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("O preço mensal deve ser maior ou igual a zero.")
            .When(x => x.MonthlyPrice.HasValue);

        RuleFor(x => x.AnnualPrice)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("O preço anual deve ser maior ou igual a zero.")
            .When(x => x.AnnualPrice.HasValue);

        RuleFor(x => x.MaxUsers)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O limite de usuários deve ser maior ou igual a zero.")
            .When(x => x.MaxUsers.HasValue);

        RuleFor(x => x.MaxQuotesPerMonth)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O limite de cotações deve ser maior ou igual a zero.")
            .When(x => x.MaxQuotesPerMonth.HasValue);
    }
}
