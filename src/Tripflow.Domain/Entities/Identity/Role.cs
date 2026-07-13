using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Identity;

public sealed class Role : AuditableEntity, ITenantEntity
{
    private Role() { }

    public Role(Guid tenantId, string name, string? description, bool isSystemRole, string createdBy)
    {
        TenantId = tenantId;
        Name = name;
        Description = description;
        IsSystemRole = isSystemRole;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool IsSystemRole { get; private set; }

    public List<RolePermission> RolePermissions = [];
    public List<UserRole> UserRoles = [];
}

