using FluentValidation;
using Tripflow.Application.DTOs.Requests.Proposals;

namespace Tripflow.Application.Validators.Proposals;

public sealed class UpdateProposalRequestValidator : AbstractValidator<UpdateProposalRequest>
{
    public UpdateProposalRequestValidator()
    {
        RuleFor(x => x.ExpiresAtUtc)
            .Must(BeInFuture!)
            .WithMessage("A data de expiração deve ser futura.")
            .When(x => x.ExpiresAtUtc.HasValue);
    }

    private static bool BeInFuture(DateTime? expiresAtUtc)
        => !expiresAtUtc.HasValue || expiresAtUtc.Value > DateTime.UtcNow;
}
