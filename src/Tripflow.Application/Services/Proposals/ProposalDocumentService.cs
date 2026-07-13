using Tripflow.Application.Services.Proposals;
using Tripflow.Domain.Entities.Proposals;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Storage;

namespace Tripflow.Application.Services.Proposals;

public interface IProposalDocumentService
{
    Task<ProposalVersion> GenerateVersionAsync(
        Proposal proposal,
        bool generatePdf,
        Guid? generatedByUserId,
        string updatedBy,
        CancellationToken cancellationToken = default);
}

public class ProposalDocumentService(
    IProposalRepository proposalRepository,
    IProposalVersionRepository versionRepository,
    IProposalTemplateRenderer templateRenderer,
    IProposalPdfGenerator pdfGenerator,
    IFileStorageService fileStorage,
    IProposalEventService eventService) : IProposalDocumentService
{
    public async Task<ProposalVersion> GenerateVersionAsync(
        Proposal proposal,
        bool generatePdf,
        Guid? generatedByUserId,
        string updatedBy,
        CancellationToken cancellationToken = default)
    {
        var versionNumber = await versionRepository.GetNextVersionNumberAsync(proposal.Id, proposal.TenantId, cancellationToken);
        var html = await templateRenderer.RenderHtmlAsync(proposal.Id, proposal.TenantId, cancellationToken);

        string? pdfUrl = null;
        if (generatePdf)
        {
            var pdfStream = await pdfGenerator.TryGeneratePdfAsync(html, cancellationToken);
            if (pdfStream is not null)
            {
                await using (pdfStream)
                {
                    var folder = $"proposals/{proposal.TenantId}/{proposal.Id}";
                    var fileName = $"proposal-v{versionNumber}.pdf";
                    pdfUrl = await fileStorage.UploadAsync(pdfStream, folder, fileName, "application/pdf", cancellationToken);
                }
            }
        }

        var version = new ProposalVersion(
            proposal.TenantId,
            proposal.Id,
            versionNumber,
            html,
            pdfUrl,
            generatedByUserId,
            updatedBy);

        await versionRepository.AddAsync(version, cancellationToken);

        if (!string.IsNullOrWhiteSpace(pdfUrl))
            proposal.SetPdfUrl(pdfUrl, updatedBy);
        else if (proposal.Status == ProposalStatus.Draft)
            proposal.MarkAsGenerated(updatedBy);

        await proposalRepository.UpdateAsync(proposal, cancellationToken);

        await eventService.RegisterAsync(proposal.TenantId, proposal.Id, ProposalEventType.Generated, $"Versão {versionNumber} gerada", cancellationToken: cancellationToken);

        return version;
    }
}
