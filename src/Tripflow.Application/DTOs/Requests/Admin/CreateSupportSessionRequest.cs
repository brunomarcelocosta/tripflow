namespace Tripflow.Application.DTOs.Requests.Admin;

public sealed class CreateSupportSessionRequest
{
    public Guid TenantId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
