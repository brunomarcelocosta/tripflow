namespace Tripflow.Application.DTOs.Responses.Roles;

public sealed class RoleResponse
{
    public Guid RoleId { get; init; }
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
    public bool IsSystemRole { get; init; }
    public IReadOnlyList<string> Permissions { get; init; } = [];
}
