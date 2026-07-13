using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Responses.Users;

public sealed class InviteUserResponse
{
    public Guid UserProfileId { get; init; }
    public string IdentityProviderUserId { get; init; } = default!;
    public string FullName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public UserStatus Status { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = [];
    public bool InviteEmailSent { get; init; }
}
