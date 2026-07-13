namespace Tripflow.Domain.Constants;

public static class PlanFeatureCodes
{
    public const string Crm = "crm";
    public const string Quotes = "quotes";
    public const string Pricing = "pricing";
    public const string ProposalPdf = "proposal_pdf";
    public const string CustomerPortal = "customer_portal";
    public const string Payments = "payments";
    public const string Branding = "branding";
    public const string Miles = "miles";
    public const string Reports = "reports";
    public const string WhiteLabel = "white_label";

    public static readonly IReadOnlyDictionary<string, string> Labels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        [Crm] = "CRM de clientes",
        [Quotes] = "Cotações e propostas",
        [Pricing] = "Precificação avançada",
        [ProposalPdf] = "PDF de propostas",
        [CustomerPortal] = "Portal do cliente",
        [Payments] = "Links de pagamento",
        [Branding] = "Identidade visual",
        [Miles] = "Gestão de milhas",
        [Reports] = "Relatórios",
        [WhiteLabel] = "White label"
    };

    public static string GetLabel(string code)
        => Labels.TryGetValue(code, out var label) ? label : code;
}
