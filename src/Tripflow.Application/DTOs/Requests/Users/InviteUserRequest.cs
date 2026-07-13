namespace Tripflow.Application.DTOs.Requests.Users;

public sealed class InviteUserRequest
{
    public string FullName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string? Phone { get; init; }
    public List<Guid>? RoleIds { get; init; }
    public List<string>? RoleNames { get; init; }
    public bool SendInviteEmail { get; init; }
}
