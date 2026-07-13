using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Identity;

public sealed class UserProfile : AuditableEntity, ITenantEntity
{
    private UserProfile() { }

    public UserProfile(Guid tenantId, string identityProviderUserId, string fullName, string email, string? phone, UserStatus status, string createdBy)
    {
        TenantId = tenantId;
        IdentityProviderUserId = identityProviderUserId;
        FullName = fullName;
        Email = email;
        Phone = phone;
        Status = status;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public string IdentityProviderUserId { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? Phone { get; private set; }
    public UserStatus Status { get; private set; }

    public List<UserRole> UserRoles = [];

    public void Activate(string updatedBy)
    {
        Status = UserStatus.Active;
        SetUpdated(updatedBy);
    }

    public void Block(string updatedBy)
    {
        Status = UserStatus.Blocked;
        SetUpdated(updatedBy);
    }

    public void Remove(string updatedBy)
    {
        Status = UserStatus.Removed;
        SetUpdated(updatedBy);
    }

    public void MarkAsInvited(string updatedBy)
    {
        Status = UserStatus.Invited;
        SetUpdated(updatedBy);
    }

    public void UpdateProfile(string fullName, string email, string? phone, UserStatus status, string updatedBy)
    {
        FullName = fullName;
        Email = email;
        Phone = phone;
        Status = status;
        SetUpdated(updatedBy);
    }

    public void AddRole(Guid roleId)
    {
        if (UserRoles.Any(x => x.RoleId == roleId))
            return;

        UserRoles.Add(new UserRole(Id, roleId));
    }

    public void RemoveRole(Guid roleId)
    {
        var userRole = UserRoles.FirstOrDefault(x => x.RoleId == roleId);

        if (userRole is not null)
            UserRoles.Remove(userRole);
    }

    public bool CanAccessPlatform()
    {
        return Status == UserStatus.Active;
    }
}

