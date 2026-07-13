namespace Tripflow.Application.Options;

public sealed class FrontendOptions
{
    public const string SectionName = "Frontend";

    public string? PublicProposalBaseUrl { get; set; }

    public string? PublicSiteBaseUrl { get; set; }
}
