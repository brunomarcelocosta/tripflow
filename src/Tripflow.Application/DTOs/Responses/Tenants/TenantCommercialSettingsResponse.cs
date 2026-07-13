namespace Tripflow.Application.DTOs.Responses.Tenants;

public sealed class TenantCommercialSettingsResponse
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public string? CommercialEmail { get; init; }
    public string? CommercialPhone { get; init; }
    public string? WhatsApp { get; init; }
    public string? Instagram { get; init; }
    public string? Website { get; init; }
    public string? DefaultTerms { get; init; }
    public string? DefaultImportantNotes { get; init; }
    public int DefaultProposalExpirationHours { get; init; }
    public DateTime? CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
