using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Proposals;
using Tripflow.Application.Services.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.UseCases.Proposals;

public class ApprovePublicProposalUseCase(
    IProposalRepository proposalRepository,
    IQuoteRepository quoteRepository,
    IProposalEventService eventService,
    ILogger<ApprovePublicProposalUseCase> logger) : IApprovePublicProposalUseCase
{
    public async Task<Result<PublicProposalActionResponse?>> ExecuteAsync(string publicToken, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(publicToken))
            return Result<PublicProposalActionResponse?>.Failure("Token inválido.");

        var validation = await ValidateAsync(publicToken, ipAddress, userAgent, cancellationToken);
        if (validation.Error is not null)
            return Result<PublicProposalActionResponse?>.Failure(validation.Error);

        var proposal = validation.Proposal!;

        try
        {
            proposal.Approve("public");
            await proposalRepository.UpdateAsync(proposal, cancellationToken);

            var quote = await quoteRepository.GetTrackedByIdAndTenantAsync(proposal.QuoteId, proposal.TenantId, cancellationToken);
            if (quote is not null && quote.Status is not (QuoteStatus.Paid or QuoteStatus.Issued or QuoteStatus.Cancelled))
            {
                quote.Approve("public");
                await quoteRepository.UpdateAsync(quote, cancellationToken);
            }

            await eventService.RegisterAsync(
                proposal.TenantId, proposal.Id, ProposalEventType.Approved,
                "Proposta aprovada pelo cliente", ipAddress, userAgent, cancellationToken);

            return Result<PublicProposalActionResponse?>.Ok(new PublicProposalActionResponse(
                proposal.Id, proposal.Status, proposal.ApprovedAtUtc, proposal.RejectedAtUtc));
        }
        catch (Exception ex)
        {
            logger.LogError("ApprovePublicProposalUseCase | Erro | {Message}", ex.Message);
            return Result<PublicProposalActionResponse?>.Failure("Erro inesperado ao aprovar proposta.");
        }
    }

    private async Task<(Domain.Entities.Proposals.Proposal? Proposal, string? Error)> ValidateAsync(
        string publicToken, string? ipAddress, string? userAgent, CancellationToken cancellationToken)
    {
        var proposal = await proposalRepository.GetTrackedByPublicTokenAsync(publicToken, cancellationToken);
        if (proposal is null)
            return (null, "Proposta não encontrada.");

        if (proposal.Status == ProposalStatus.Cancelled)
            return (null, "Proposta cancelada.");

        if (proposal.Status is ProposalStatus.Approved or ProposalStatus.Rejected)
            return (null, "Proposta já foi respondida.");

        if (proposal.Status == ProposalStatus.Expired)
            return (null, "Proposta expirada.");

        if (proposal.IsExpired(DateTime.UtcNow))
        {
            proposal.Expire("system");
            await proposalRepository.UpdateAsync(proposal, cancellationToken);
            await eventService.RegisterAsync(proposal.TenantId, proposal.Id, ProposalEventType.Expired, "Proposta expirada automaticamente", ipAddress, userAgent, cancellationToken);
            return (null, "Proposta expirada.");
        }

        return (proposal, null);
    }
}
