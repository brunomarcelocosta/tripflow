using Microsoft.AspNetCore.Authorization;

namespace Tripflow.Infra.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequirePermissionAttribute(string permissionCode)
    : AuthorizeAttribute, IAuthorizationRequirementData
{
    public string PermissionCode { get; } = permissionCode;

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return new PermissionRequirement(PermissionCode);
    }
}
