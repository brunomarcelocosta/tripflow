namespace Tripflow.Application.DTOs.Requests.Roles;

public sealed class AssignUserRolesRequest
{
    public List<Guid>? RoleIds { get; init; }
    public List<string>? RoleNames { get; init; }
}
