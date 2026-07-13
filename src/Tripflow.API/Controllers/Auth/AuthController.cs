using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.API.Controllers.Auth;

[ApiController]
[Route("api/auth")]
[Authorize]
public sealed class AuthController(IUserContext userContext, ITenantContext tenantContext) : ControllerBase
{
    [HttpGet("me")]
    [AllowAnonymous]
    public IActionResult GetMe()
    {
        if (!userContext.IsAuthenticated)
        {
            return Ok(new
            {
                success = true,
                data = new
                {
                    IsAuthenticated = false,
                    IdentityProviderUserId = (string?)null,
                    Email = (string?)null,
                    Name = (string?)null,
                    Roles = Array.Empty<string>(),
                    TenantId = (Guid?)null
                },
                error = (string?)null
            });
        }

        return Ok(new
        {
            success = true,
            data = new
            {
                IsAuthenticated = true,
                userContext.IdentityProviderUserId,
                userContext.Email,
                userContext.Name,
                userContext.Roles,
                tenantContext.TenantId
            },
            error = (string?)null
        });
    }
}
