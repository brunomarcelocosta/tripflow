using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.Domain.Entities.Identity;

public sealed class UserInvitation : AuditableEntity, ITenantEntity
{
    private UserInvitation() { }

    public UserInvitation(Guid tenantId, string email, string fullName, string tokenHash, DateTime expiresAtUtc, string createdBy)
    {
        TenantId = tenantId;
        Email = email;
        FullName = fullName;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
        SetCreated(createdBy);
    }

    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    public string Email { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public string TokenHash { get; private set; } = default!;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? AcceptedAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
}

