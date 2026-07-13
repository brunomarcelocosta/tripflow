using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.Services.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Proposals;

public class MarkProposalAsSentUseCase(
    IProposalRepository proposalRepository,
    IProposalEventService eventService,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<MarkProposalAsSentUseCase> logger) : IMarkProposalAsSentUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.ProposalsSend, cancellationToken))
            return Result<bool>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var proposal = await proposalRepository.GetTrackedByIdAndTenantAsync(id, tenantId, cancellationToken);
        if (proposal is null)
            return Result<bool>.Failure("Proposta não encontrada.");

        try
        {
            proposal.MarkAsSent(updatedBy);
            await proposalRepository.UpdateAsync(proposal, cancellationToken);
            await eventService.RegisterAsync(tenantId, id, ProposalEventType.Sent, "Proposta marcada como enviada", cancellationToken: cancellationToken);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("MarkProposalAsSentUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao marcar proposta como enviada.");
        }
    }
}
