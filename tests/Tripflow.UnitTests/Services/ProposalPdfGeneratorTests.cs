using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Tripflow.Application.Options;
using Tripflow.Application.Services.Proposals;

namespace Tripflow.UnitTests.Services;

public class ProposalPdfGeneratorTests
{
    [Fact]
    public async Task TryGeneratePdfAsync_Should_ReturnNull_When_Disabled()
    {
        var generator = new ProposalPdfGenerator(
            Options.Create(new PdfOptions { Enabled = false }),
            NullLogger<ProposalPdfGenerator>.Instance);

        var result = await generator.TryGeneratePdfAsync("<html><body><h1>Test</h1></body></html>");

        Assert.Null(result);
    }
}
