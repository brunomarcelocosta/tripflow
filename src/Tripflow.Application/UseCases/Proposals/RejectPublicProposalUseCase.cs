using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Proposals;
using Tripflow.Application.DTOs.Responses.Proposals;
using Tripflow.Application.Services.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.UseCases.Proposals;

public class RejectPublicProposalUseCase(
    IProposalRepository proposalRepository,
    IQuoteRepository quoteRepository,
    IProposalEventService eventService,
    IValidator<RejectPublicProposalRequest> validator,
    ILogger<RejectPublicProposalUseCase> logger) : IRejectPublicProposalUseCase
{
    public async Task<Result<PublicProposalActionResponse?>> ExecuteAsync(
        string publicToken, RejectPublicProposalRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(publicToken))
            return Result<PublicProposalActionResponse?>.Failure("Token inválido.");

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result<PublicProposalActionResponse?>.Failure(validationResult.Errors.First().ErrorMessage);

        var proposal = await proposalRepository.GetTrackedByPublicTokenAsync(publicToken, cancellationToken);
        if (proposal is null)
            return Result<PublicProposalActionResponse?>.Failure("Proposta não encontrada.");

        if (proposal.Status == ProposalStatus.Cancelled)
            return Result<PublicProposalActionResponse?>.Failure("Proposta cancelada.");

        if (proposal.Status is ProposalStatus.Approved or ProposalStatus.Rejected)
            return Result<PublicProposalActionResponse?>.Failure("Proposta já foi respondida.");

        if (proposal.Status == ProposalStatus.Expired)
            return Result<PublicProposalActionResponse?>.Failure("Proposta expirada.");

        if (proposal.IsExpired(DateTime.UtcNow))
        {
            proposal.Expire("system");
            await proposalRepository.UpdateAsync(proposal, cancellationToken);
            await eventService.RegisterAsync(proposal.TenantId, proposal.Id, ProposalEventType.Expired, "Proposta expirada automaticamente", ipAddress, userAgent, cancellationToken);
            return Result<PublicProposalActionResponse?>.Failure("Proposta expirada.");
        }

        try
        {
            proposal.Reject("public");
            await proposalRepository.UpdateAsync(proposal, cancellationToken);

            var quote = await quoteRepository.GetTrackedByIdAndTenantAsync(proposal.QuoteId, proposal.TenantId, cancellationToken);
            if (quote is not null && quote.Status is not (QuoteStatus.Paid or QuoteStatus.Issued or QuoteStatus.Cancelled))
            {
                quote.Reject("public");
                await quoteRepository.UpdateAsync(quote, cancellationToken);
            }

            var description = string.IsNullOrWhiteSpace(request.Reason)
                ? "Proposta rejeitada pelo cliente"
                : $"Proposta rejeitada: {request.Reason.Trim()}";

            await eventService.RegisterAsync(
                proposal.TenantId, proposal.Id, ProposalEventType.Rejected,
                description, ipAddress, userAgent, cancellationToken);

            return Result<PublicProposalActionResponse?>.Ok(new PublicProposalActionResponse(
                proposal.Id, proposal.Status, proposal.ApprovedAtUtc, proposal.RejectedAtUtc));
        }
        catch (Exception ex)
        {
            logger.LogError("RejectPublicProposalUseCase | Erro | {Message}", ex.Message);
            return Result<PublicProposalActionResponse?>.Failure("Erro inesperado ao rejeitar proposta.");
        }
    }
}
