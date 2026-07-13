using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Proposals;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Proposals;

namespace Tripflow.Application.UseCases.Proposals.Interfaces;

public interface IGetProposalsUseCase
{
    Task<Result<PagedResponse<ProposalResponse>>> ExecuteAsync(ProposalFilterRequest request, CancellationToken cancellationToken = default);
}

public interface IGetProposalByIdUseCase
{
    Task<Result<ProposalResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICreateProposalFromQuoteUseCase
{
    Task<Result<ProposalResponse?>> ExecuteAsync(Guid quoteId, CreateProposalFromQuoteRequest request, CancellationToken cancellationToken = default);
}

public interface IUpdateProposalUseCase
{
    Task<Result<ProposalResponse?>> ExecuteAsync(Guid id, UpdateProposalRequest request, CancellationToken cancellationToken = default);
}

public interface IDeleteProposalUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICancelProposalUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IMarkProposalAsSentUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IExpireProposalUseCase
{
    Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IGetProposalVersionsUseCase
{
    Task<Result<IEnumerable<ProposalVersionResponse>>> ExecuteAsync(Guid proposalId, CancellationToken cancellationToken = default);
}

public interface IGetProposalVersionByIdUseCase
{
    Task<Result<ProposalVersionResponse?>> ExecuteAsync(Guid proposalId, Guid versionId, CancellationToken cancellationToken = default);
}

public interface IGenerateProposalVersionUseCase
{
    Task<Result<ProposalVersionResponse?>> ExecuteAsync(Guid proposalId, GenerateProposalVersionRequest request, CancellationToken cancellationToken = default);
}

public interface IGenerateProposalPdfUseCase
{
    Task<Result<ProposalVersionResponse?>> ExecuteAsync(Guid proposalId, CancellationToken cancellationToken = default);
}

public interface IRegenerateProposalPublicLinkUseCase
{
    Task<Result<ProposalResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IGetProposalEventsUseCase
{
    Task<Result<IEnumerable<ProposalEventResponse>>> ExecuteAsync(Guid proposalId, CancellationToken cancellationToken = default);
}

public interface IGetPublicProposalByTokenUseCase
{
    Task<Result<PublicProposalResponse?>> ExecuteAsync(string publicToken, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default);
}

public interface IApprovePublicProposalUseCase
{
    Task<Result<PublicProposalActionResponse?>> ExecuteAsync(string publicToken, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default);
}

public interface IRejectPublicProposalUseCase
{
    Task<Result<PublicProposalActionResponse?>> ExecuteAsync(string publicToken, RejectPublicProposalRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default);
}
