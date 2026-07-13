using Microsoft.AspNetCore.Http;
using Tripflow.Domain.Entities.Audit;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.Services.Audit;

public sealed class AuditService(
    IAuditLogRepository auditLogRepository,
    IUserContext userContext,
    IUserProfileRepository userProfileRepository,
    IHttpContextAccessor httpContextAccessor) : IAuditService
{
    public async Task RegisterAsync(AuditLogRequest request, CancellationToken cancellationToken = default)
    {
        var tenantId = request.TenantId ?? TripflowDbSeedData.PlatformTenantId;

        Guid? userId = request.UserProfileId;

        if (userId is null &&
            !string.IsNullOrWhiteSpace(userContext.IdentityProviderUserId))
        {
            var profile = await userProfileRepository.GetByIdentityProviderUserIdAsync(
                userContext.IdentityProviderUserId,
                cancellationToken);

            userId = profile?.Id;
        }

        var httpContext = httpContextAccessor.HttpContext;
        var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext?.Request.Headers["User-Agent"].FirstOrDefault();

        var auditLog = new AuditLog(
            tenantId,
            userId,
            request.Action,
            request.EntityName,
            request.EntityId,
            oldValuesJson: null,
            newValuesJson: request.Description,
            ipAddress,
            userAgent,
            correlationId: null);

        await auditLogRepository.AddAsync(auditLog, cancellationToken);
    }
}
