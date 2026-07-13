using FluentValidation;
using Tripflow.Application.DTOs.Requests.Proposals;

namespace Tripflow.Application.Validators.Proposals;

public sealed class GenerateProposalVersionRequestValidator : AbstractValidator<GenerateProposalVersionRequest>
{
    public GenerateProposalVersionRequestValidator()
    {
    }
}
