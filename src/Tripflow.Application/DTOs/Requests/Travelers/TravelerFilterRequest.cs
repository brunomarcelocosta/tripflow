namespace Tripflow.Application.DTOs.Requests.Travelers;

public sealed class TravelerFilterRequest : FilterRequest
{
    public string? FullName { get; set; }
    public string? DocumentNumber { get; set; }
    public string? PassportNumber { get; set; }
    public string? Nationality { get; set; }
    public DateOnly? PassportExpiringBefore { get; set; }
}
