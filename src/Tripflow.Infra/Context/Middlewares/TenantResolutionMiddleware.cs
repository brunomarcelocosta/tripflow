using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Contexts;

namespace Tripflow.Infra.Context.Middlewares;

public sealed class TenantResolutionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, IUserContext userContext, ITenantContextSetter tenantContext, TripflowDbContext dbContext)
    {
        if (!userContext.IsAuthenticated ||
            string.IsNullOrWhiteSpace(userContext.IdentityProviderUserId))
        {
            tenantContext.Clear();
            await next(httpContext);
            return;
        }

        var tenantId = await dbContext.UserProfiles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.IdentityProviderUserId == userContext.IdentityProviderUserId)
            .Select(x => x.TenantId)
            .FirstOrDefaultAsync();

        if (tenantId == Guid.Empty)
        {
            tenantContext.Clear();
            await next(httpContext);
            return;
        }

        tenantContext.SetTenant(tenantId);

        await next(httpContext);
    }
}