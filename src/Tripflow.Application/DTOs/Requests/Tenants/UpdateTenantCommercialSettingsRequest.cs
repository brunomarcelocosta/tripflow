namespace Tripflow.Application.DTOs.Requests.Tenants;

public sealed record UpdateTenantCommercialSettingsRequest
{
    public string? CommercialEmail { get; init; }
    public string? CommercialPhone { get; init; }
    public string? WhatsApp { get; init; }
    public string? Instagram { get; init; }
    public string? Website { get; init; }
    public string? DefaultTerms { get; init; }
    public string? DefaultImportantNotes { get; init; }
    public int DefaultProposalExpirationHours { get; init; } = 24;
}
