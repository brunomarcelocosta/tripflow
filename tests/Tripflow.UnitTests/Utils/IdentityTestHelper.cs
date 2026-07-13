using System.Reflection;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;

namespace Tripflow.UnitTests.Utils;

public static class IdentityTestHelper
{
    public static UserProfile CreateUserProfile(
        Guid? tenantId = null,
        string identityProviderUserId = "keycloak-user-123",
        string fullName = "Usuário Teste",
        string email = "user@test.com",
        UserStatus status = UserStatus.Active,
        Tenant? tenant = null)
    {
        tenant ??= TenantTestHelper.Create(tradeName: "Empresa Teste");
        var resolvedTenantId = tenantId ?? tenant.Id;

        var profile = new UserProfile(
            resolvedTenantId,
            identityProviderUserId,
            fullName,
            email,
            null,
            status,
            "system");

        SetNavigation(profile, nameof(UserProfile.Tenant), tenant);

        return profile;
    }

    public static Role CreateRole(
        Guid tenantId,
        string name = "Consultant",
        string? description = "Consultor",
        bool isSystemRole = true,
        params Permission[] permissions)
    {
        var role = new Role(tenantId, name, description, isSystemRole, "system");

        foreach (var permission in permissions)
            role.RolePermissions.Add(new RolePermission(role.Id, permission.Id));

        foreach (var rolePermission in role.RolePermissions)
            SetNavigation(rolePermission, nameof(RolePermission.Permission), permissions.First(p => p.Id == rolePermission.PermissionId));

        return role;
    }

    public static Permission CreatePermission(string code = "users.read", string? description = null) =>
        new(code, description ?? code);

    public static void AddRoleToUser(UserProfile profile, Role role)
    {
        profile.AddRole(role.Id);
        var userRole = profile.UserRoles.Single(x => x.RoleId == role.Id);
        SetNavigation(userRole, nameof(UserRole.Role), role);
    }

    private static void SetNavigation(object entity, string propertyName, object value)
    {
        var property = entity.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property?.SetValue(entity, value);
    }
}
