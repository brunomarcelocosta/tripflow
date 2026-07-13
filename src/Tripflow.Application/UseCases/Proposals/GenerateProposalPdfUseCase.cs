using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Proposals;
using Tripflow.Application.DTOs.Responses.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;

namespace Tripflow.Application.UseCases.Proposals;

public class GenerateProposalPdfUseCase(IGenerateProposalVersionUseCase generateVersionUseCase) : IGenerateProposalPdfUseCase
{
    public Task<Result<ProposalVersionResponse?>> ExecuteAsync(Guid proposalId, CancellationToken cancellationToken = default)
        => generateVersionUseCase.ExecuteAsync(proposalId, new GenerateProposalVersionRequest(GeneratePdf: true), cancellationToken);
}
