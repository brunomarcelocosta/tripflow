using Microsoft.AspNetCore.Authorization;

namespace Tripflow.Infra.Auth;

public sealed class PermissionRequirement(string permissionCode) : IAuthorizationRequirement
{
    public string PermissionCode { get; } = permissionCode;
}
