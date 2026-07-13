namespace Tripflow.Application.Services.Proposals;

public interface IProposalTemplateRenderer
{
    Task<string> RenderHtmlAsync(Guid proposalId, Guid tenantId, CancellationToken cancellationToken = default);
}
