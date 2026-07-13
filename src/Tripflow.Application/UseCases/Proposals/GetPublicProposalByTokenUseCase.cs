using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Proposals;
using Tripflow.Application.Helpers;
using Tripflow.Application.Services.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Application.UseCases.Proposals;

public class GetPublicProposalByTokenUseCase(
    IProposalRepository proposalRepository,
    IProposalEventService eventService,
    ILogger<GetPublicProposalByTokenUseCase> logger) : IGetPublicProposalByTokenUseCase
{
    public async Task<Result<PublicProposalResponse?>> ExecuteAsync(string publicToken, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(publicToken))
            return Result<PublicProposalResponse?>.Failure("Token inválido.");

        var proposal = await proposalRepository.GetByPublicTokenAsync(publicToken, cancellationToken);
        if (proposal is null)
            return Result<PublicProposalResponse?>.Failure("Proposta não encontrada.");

        if (proposal.Status == ProposalStatus.Cancelled)
            return Result<PublicProposalResponse?>.Failure("Proposta cancelada.");

        if (proposal.Status == ProposalStatus.Expired || proposal.IsExpired(DateTime.UtcNow))
        {
            await TryExpireAsync(proposal, cancellationToken);
            return Result<PublicProposalResponse?>.Failure("Proposta expirada.");
        }

        try
        {
            if (!proposal.ViewedAtUtc.HasValue)
            {
                var tracked = await proposalRepository.GetTrackedByPublicTokenAsync(publicToken, cancellationToken);
                if (tracked is not null)
                {
                    tracked.MarkAsViewed("public");
                    await proposalRepository.UpdateAsync(tracked, cancellationToken);
                    await eventService.RegisterAsync(
                        tracked.TenantId, tracked.Id, ProposalEventType.Viewed,
                        "Proposta visualizada publicamente", ipAddress, userAgent, cancellationToken);
                    proposal = await proposalRepository.GetByPublicTokenAsync(publicToken, cancellationToken) ?? proposal;
                }
            }

            return Result<PublicProposalResponse?>.Ok(PublicProposalMapper.Map(proposal));
        }
        catch (Exception ex)
        {
            logger.LogError("GetPublicProposalByTokenUseCase | Erro | {Message}", ex.Message);
            return Result<PublicProposalResponse?>.Failure("Erro inesperado ao consultar proposta.");
        }
    }

    private async Task TryExpireAsync(Domain.Entities.Proposals.Proposal proposal, CancellationToken cancellationToken)
    {
        if (proposal.Status == ProposalStatus.Expired)
            return;

        var tracked = await proposalRepository.GetTrackedByPublicTokenAsync(proposal.PublicToken!, cancellationToken);
        if (tracked is null || tracked.Status == ProposalStatus.Expired)
            return;

        tracked.Expire("system");
        await proposalRepository.UpdateAsync(tracked, cancellationToken);
        await eventService.RegisterAsync(tracked.TenantId, tracked.Id, ProposalEventType.Expired, "Proposta expirada automaticamente", cancellationToken: cancellationToken);
    }
}
