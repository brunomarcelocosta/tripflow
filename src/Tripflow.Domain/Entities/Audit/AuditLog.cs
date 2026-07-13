using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Audit;

public sealed class AuditLog : BaseEntity, ITenantEntity
{
    private AuditLog() { }

    public AuditLog(Guid tenantId, Guid? userId, string action, string? entityName, Guid? entityId, string? oldValuesJson, string? newValuesJson, string? ipAddress, string? userAgent, string? correlationId)
    {
        TenantId = tenantId;
        UserId = userId;
        Action = action;
        EntityName = entityName;
        EntityId = entityId;
        OldValuesJson = oldValuesJson;
        NewValuesJson = newValuesJson;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        CorrelationId = correlationId;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public Guid? UserId { get; private set; }
    public UserProfile? User { get; private set; }

    public string Action { get; private set; } = default!;
    public string? EntityName { get; private set; }
    public Guid? EntityId { get; private set; }
    public string? OldValuesJson { get; private set; }
    public string? NewValuesJson { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? CorrelationId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}

