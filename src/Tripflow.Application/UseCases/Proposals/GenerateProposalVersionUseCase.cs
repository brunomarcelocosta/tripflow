using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Proposals;
using Tripflow.Application.DTOs.Responses.Proposals;
using Tripflow.Application.Services.Proposals;
using Tripflow.Application.UseCases.Proposals.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Proposals;

public class GenerateProposalVersionUseCase(
    IProposalRepository proposalRepository,
    IProposalDocumentService documentService,
    IValidator<GenerateProposalVersionRequest> validator,
    IMapper mapper,
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserPermissionService userPermissionService,
    ILogger<GenerateProposalVersionUseCase> logger) : IGenerateProposalVersionUseCase
{
    public async Task<Result<ProposalVersionResponse?>> ExecuteAsync(Guid proposalId, GenerateProposalVersionRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated || !tenantContext.HasTenant || tenantContext.TenantId is null)
            return Result<ProposalVersionResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.ProposalsWrite, cancellationToken))
            return Result<ProposalVersionResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<ProposalVersionResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var tenantId = tenantContext.TenantId.Value;
        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var proposal = await proposalRepository.GetTrackedByIdAndTenantAsync(proposalId, tenantId, cancellationToken);
        if (proposal is null)
            return Result<ProposalVersionResponse?>.Failure("Proposta não encontrada.");
        if (!proposal.CanBeUpdated())
            return Result<ProposalVersionResponse?>.Failure("Proposta não pode ser alterada no status atual.");

        try
        {
            var version = await documentService.GenerateVersionAsync(proposal, request.GeneratePdf, null, updatedBy, cancellationToken);
            return Result<ProposalVersionResponse?>.Ok(mapper.Map<ProposalVersionResponse>(version));
        }
        catch (Exception ex)
        {
            logger.LogError("GenerateProposalVersionUseCase | Erro | {Message}", ex.Message);
            return Result<ProposalVersionResponse?>.Failure("Erro inesperado ao gerar versão.");
        }
    }
}
