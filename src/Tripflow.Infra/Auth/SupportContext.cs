using Microsoft.AspNetCore.Http;
using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.Infra.Auth;

public sealed class SupportContext(IHttpContextAccessor httpContextAccessor) : ISupportContext
{
    public const string HeaderName = "X-Support-Tenant-Id";

    public Guid? TargetTenantId
    {
        get
        {
            var header = httpContextAccessor.HttpContext?.Request.Headers[HeaderName].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(header))
                return null;

            return Guid.TryParse(header, out var tenantId) ? tenantId : null;
        }
    }

    public bool IsSupportMode => TargetTenantId.HasValue;
}
