namespace Tripflow.Application.DTOs.Requests;

public class FilterRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? SortBy { get; set; } = "CreatedAtUtc";
    public bool SortDesc { get; set; } = true;

    public string? Search { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
}
