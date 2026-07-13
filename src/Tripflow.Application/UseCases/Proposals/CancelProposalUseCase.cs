using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.Services.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Proposals;

public class CancelProposalUseCase(
    IProposalRepository proposalRepository,
    IProposalEventService eventService,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<CancelProposalUseCase> logger) : ICancelProposalUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<bool>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.ProposalsWrite, cancellationToken))
            return Result<bool>.Forbidden();

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var proposal = await proposalRepository.GetTrackedByIdAndTenantAsync(id, tenantId, cancellationToken);
        if (proposal is null)
            return Result<bool>.Failure("Proposta não encontrada.");
        if (!proposal.CanBeCancelled())
            return Result<bool>.Failure("Proposta não pode ser cancelada no status atual.");

        try
        {
            proposal.Cancel(updatedBy);
            await proposalRepository.UpdateAsync(proposal, cancellationToken);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError("CancelProposalUseCase | Erro | {Message}", ex.Message);
            return Result<bool>.Failure("Erro inesperado ao cancelar proposta.");
        }
    }
}
