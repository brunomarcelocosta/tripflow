namespace Tripflow.Application.Options;

public sealed class PdfOptions
{
    public const string SectionName = "Pdf";

    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Optional folder for Chromium download. Empty uses default cache.
    /// </summary>
    public string? BrowserDownloadPath { get; init; }
}
