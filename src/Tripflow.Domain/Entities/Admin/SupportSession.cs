using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Tenants;

namespace Tripflow.Domain.Entities.Admin;

public sealed class SupportSession : BaseEntity
{
    private SupportSession() { }

    public SupportSession(
        Guid adminUserProfileId,
        string adminIdentityProviderUserId,
        Guid targetTenantId,
        string reason,
        string createdBy)
    {
        AdminUserProfileId = adminUserProfileId;
        AdminIdentityProviderUserId = adminIdentityProviderUserId;
        TargetTenantId = targetTenantId;
        Reason = reason;
        StartedAtUtc = DateTime.UtcNow;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public Guid AdminUserProfileId { get; private set; }
    public UserProfile AdminUserProfile { get; private set; } = default!;

    public string AdminIdentityProviderUserId { get; private set; } = default!;

    public Guid TargetTenantId { get; private set; }
    public Tenant TargetTenant { get; private set; } = default!;

    public string Reason { get; private set; } = default!;
    public DateTime StartedAtUtc { get; private set; }
    public DateTime? EndedAtUtc { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string CreatedBy { get; private set; } = default!;

    public void End(string endedBy)
    {
        IsActive = false;
        EndedAtUtc = DateTime.UtcNow;
    }
}
