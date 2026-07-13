using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using AppPdfOptions = Tripflow.Application.Options.PdfOptions;

namespace Tripflow.Application.Services.Proposals;

public class ProposalPdfGenerator(
    IOptions<AppPdfOptions> pdfOptions,
    ILogger<ProposalPdfGenerator> logger) : IProposalPdfGenerator
{
    private static readonly SemaphoreSlim BrowserLock = new(1, 1);
    private static IBrowser? _browser;

    public async Task<Stream?> TryGeneratePdfAsync(string html, CancellationToken cancellationToken = default)
    {
        if (!pdfOptions.Value.Enabled)
            return null;

        if (string.IsNullOrWhiteSpace(html))
            return null;

        try
        {
            var browser = await GetBrowserAsync(cancellationToken);
            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(html, new NavigationOptions { WaitUntil = [WaitUntilNavigation.Networkidle0] });

            var pdfBytes = await page.PdfDataAsync(new PuppeteerSharp.PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions
                {
                    Top = "12mm",
                    Bottom = "12mm",
                    Left = "10mm",
                    Right = "10mm"
                }
            });

            return new MemoryStream(pdfBytes);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "ProposalPdfGenerator | Falha ao gerar PDF a partir do HTML");
            return null;
        }
    }

    private async Task<IBrowser> GetBrowserAsync(CancellationToken cancellationToken)
    {
        if (_browser is { IsConnected: true })
            return _browser;

        await BrowserLock.WaitAsync(cancellationToken);
        try
        {
            if (_browser is { IsConnected: true })
                return _browser;

            var downloadPath = pdfOptions.Value.BrowserDownloadPath;
            if (!string.IsNullOrWhiteSpace(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
                var fetcher = new BrowserFetcher(new BrowserFetcherOptions { Path = downloadPath });
                await fetcher.DownloadAsync();
            }
            else
            {
                var fetcher = new BrowserFetcher();
                await fetcher.DownloadAsync();
            }

            _browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = ["--no-sandbox", "--disable-setuid-sandbox", "--disable-dev-shm-usage"]
            });

            return _browser;
        }
        finally
        {
            BrowserLock.Release();
        }
    }
}
