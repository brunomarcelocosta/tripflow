namespace Tripflow.Application.Services.Proposals;

public interface IProposalPdfGenerator
{
    Task<Stream?> TryGeneratePdfAsync(string html, CancellationToken cancellationToken = default);
}
