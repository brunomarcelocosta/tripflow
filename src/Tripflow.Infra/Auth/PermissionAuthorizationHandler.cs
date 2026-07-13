using Microsoft.AspNetCore.Authorization;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Infra.Auth;

public sealed class PermissionAuthorizationHandler(IUserPermissionService permissionService)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (await permissionService.HasPermissionAsync(requirement.PermissionCode))
            context.Succeed(requirement);
    }
}
