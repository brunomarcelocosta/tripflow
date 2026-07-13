using FluentValidation;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Domain.Enums;

namespace Tripflow.Application.Validators.Miles;

public sealed class CreateLoyaltyProgramRequestValidator : AbstractValidator<CreateLoyaltyProgramRequest>
{
    public CreateLoyaltyProgramRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do programa é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do programa deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("O país deve ter no máximo 100 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Country));

        RuleFor(x => x.AirlineName)
            .MaximumLength(150).WithMessage("O nome da companhia aérea deve ter no máximo 150 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.AirlineName));

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("O status do programa é inválido.")
            .When(x => x.Status.HasValue);
    }
}

public sealed class UpdateLoyaltyProgramRequestValidator : AbstractValidator<UpdateLoyaltyProgramRequest>
{
    public UpdateLoyaltyProgramRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do programa é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do programa deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("O país deve ter no máximo 100 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Country));

        RuleFor(x => x.AirlineName)
            .MaximumLength(150).WithMessage("O nome da companhia aérea deve ter no máximo 150 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.AirlineName));

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("O status do programa é inválido.");
    }
}

public sealed class CreateCustomerLoyaltyAccountRequestValidator : AbstractValidator<CreateCustomerLoyaltyAccountRequest>
{
    public CreateCustomerLoyaltyAccountRequestValidator()
    {
        RuleFor(x => x.LoyaltyProgramId)
            .NotEmpty().WithMessage("O programa de fidelidade é obrigatório.");

        RuleFor(x => x.AccountNumber)
            .MaximumLength(100).WithMessage("O número da conta deve ter no máximo 100 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.AccountNumber));

        RuleFor(x => x.CurrentBalance)
            .GreaterThanOrEqualTo(0).WithMessage("O saldo atual não pode ser negativo.");

        RuleFor(x => x.AverageCostPerThousand)
            .GreaterThanOrEqualTo(0).WithMessage("O custo médio por mil não pode ser negativo.")
            .When(x => x.AverageCostPerThousand.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(4000).WithMessage("As observações devem ter no máximo 4000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("O status da conta é inválido.")
            .When(x => x.Status.HasValue);
    }
}

public sealed class UpdateCustomerLoyaltyAccountRequestValidator : AbstractValidator<UpdateCustomerLoyaltyAccountRequest>
{
    public UpdateCustomerLoyaltyAccountRequestValidator()
    {
        RuleFor(x => x.LoyaltyProgramId)
            .NotEmpty().WithMessage("O programa de fidelidade é obrigatório.");

        RuleFor(x => x.AccountNumber)
            .MaximumLength(100).WithMessage("O número da conta deve ter no máximo 100 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.AccountNumber));

        RuleFor(x => x.CurrentBalance)
            .GreaterThanOrEqualTo(0).WithMessage("O saldo atual não pode ser negativo.");

        RuleFor(x => x.AverageCostPerThousand)
            .GreaterThanOrEqualTo(0).WithMessage("O custo médio por mil não pode ser negativo.")
            .When(x => x.AverageCostPerThousand.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(4000).WithMessage("As observações devem ter no máximo 4000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("O status da conta é inválido.");
    }
}

public sealed class CreateMilesTransactionRequestValidator : AbstractValidator<CreateMilesTransactionRequest>
{
    public CreateMilesTransactionRequestValidator()
    {
        RuleFor(x => x.CustomerLoyaltyAccountId)
            .NotEmpty().WithMessage("A conta de fidelidade é obrigatória.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("O tipo de transação é inválido.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("A quantidade de milhas deve ser maior que zero.");

        RuleFor(x => x.CostPerThousand)
            .GreaterThanOrEqualTo(0).WithMessage("O custo por mil não pode ser negativo.")
            .When(x => x.CostPerThousand.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("A descrição deve ter no máximo 1000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.TransactionDateUtc)
            .Must(x => x!.Value <= DateTime.UtcNow.AddMinutes(5))
            .WithMessage("A data da transação não pode estar no futuro.")
            .When(x => x.TransactionDateUtc.HasValue);

        RuleFor(x => x.ExpiresAt)
            .NotNull().WithMessage("A data de expiração é obrigatória quando for solicitado criar lote de expiração.")
            .When(x => x.CreateExpirationBatch && x.Type == MilesTransactionType.Credit);
    }
}

public sealed class CreateMilesExpirationBatchRequestValidator : AbstractValidator<CreateMilesExpirationBatchRequest>
{
    public CreateMilesExpirationBatchRequestValidator()
    {
        RuleFor(x => x.CustomerLoyaltyAccountId)
            .NotEmpty().WithMessage("A conta de fidelidade é obrigatória.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("A quantidade de milhas deve ser maior que zero.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("O status do lote é inválido.")
            .When(x => x.Status.HasValue);
    }
}

public sealed class UpdateMilesExpirationBatchRequestValidator : AbstractValidator<UpdateMilesExpirationBatchRequest>
{
    public UpdateMilesExpirationBatchRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("A quantidade de milhas deve ser maior que zero.");
    }
}
