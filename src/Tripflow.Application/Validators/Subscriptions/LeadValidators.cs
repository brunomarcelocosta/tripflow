using FluentValidation;
using Tripflow.Application.DTOs.Requests.Subscriptions;

namespace Tripflow.Application.Validators.Subscriptions;

public sealed class CreateLeadRequestValidator : AbstractValidator<CreateLeadRequest>
{
    public CreateLeadRequestValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("O nome da empresa é obrigatório.")
            .MaximumLength(200)
            .WithMessage("O nome da empresa deve ter no máximo 200 caracteres.");

        RuleFor(x => x.ResponsibleName)
            .NotEmpty()
            .WithMessage("O nome do responsável é obrigatório.")
            .MaximumLength(200)
            .WithMessage("O nome do responsável deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("O e-mail é obrigatório.")
            .EmailAddress()
            .WithMessage("O e-mail informado é inválido.")
            .MaximumLength(200)
            .WithMessage("O e-mail deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Phone)
            .MaximumLength(50)
            .WithMessage("O telefone deve ter no máximo 50 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.PlanOfInterest)
            .MaximumLength(100)
            .WithMessage("O plano de interesse deve ter no máximo 100 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.PlanOfInterest));

        RuleFor(x => x.Message)
            .MaximumLength(4000)
            .WithMessage("A mensagem deve ter no máximo 4000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Message));
    }
}

public sealed class LeadFilterRequestValidator : AbstractValidator<LeadFilterRequest>
{
    public LeadFilterRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("A página deve ser maior que zero.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 200)
            .WithMessage("O tamanho da página deve estar entre 1 e 200.");

        RuleFor(x => x.CreatedToUtc)
            .GreaterThanOrEqualTo(x => x.CreatedFromUtc!.Value)
            .WithMessage("A data final deve ser maior ou igual à data inicial.")
            .When(x => x.CreatedFromUtc.HasValue && x.CreatedToUtc.HasValue);
    }
}

public sealed class ConvertLeadToTenantRequestValidator : AbstractValidator<ConvertLeadToTenantRequest>
{
    public ConvertLeadToTenantRequestValidator()
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
            .MaximumLength(50)
            .WithMessage("O documento deve ter no máximo 50 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.DocumentNumber));

        RuleFor(x => x.SubscriptionPlanId)
            .NotEmpty()
            .WithMessage("O plano informado é inválido.")
            .When(x => x.SubscriptionPlanId.HasValue);
    }
}
