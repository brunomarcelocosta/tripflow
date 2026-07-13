using FluentValidation;
using Tripflow.Application.DTOs.Requests.Proposals;

namespace Tripflow.Application.Validators.Proposals;

public sealed class RejectPublicProposalRequestValidator : AbstractValidator<RejectPublicProposalRequest>
{
    public RejectPublicProposalRequestValidator()
    {
        RuleFor(x => x.Reason)
            .MaximumLength(2000)
            .WithMessage("O motivo da rejeição deve ter no máximo 2000 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Reason));
    }
}
