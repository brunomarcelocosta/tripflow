namespace Tripflow.Domain.Entities.Identity;

public sealed class UserRole
{
    private UserRole() { }

    public UserRole(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public Guid UserId { get; private set; }
    public UserProfile User { get; private set; } = default!;

    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = default!;
}

