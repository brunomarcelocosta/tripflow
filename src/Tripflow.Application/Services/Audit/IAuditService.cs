namespace Tripflow.Application.Services.Audit;

public interface IAuditService
{
    Task RegisterAsync(AuditLogRequest request, CancellationToken cancellationToken = default);
}

public sealed record AuditLogRequest(
    Guid? TenantId,
    Guid? UserProfileId,
    string Action,
    string EntityName,
    Guid? EntityId,
    string? Description);

public static class AuditActions
{
    public const string TenantActivated = "TenantActivated";
    public const string TenantSuspended = "TenantSuspended";
    public const string TenantCancelled = "TenantCancelled";
    public const string UserActivated = "UserActivated";
    public const string UserBlocked = "UserBlocked";
    public const string UserRemoved = "UserRemoved";
    public const string UserUpdated = "UserUpdated";
    public const string UserPasswordChanged = "UserPasswordChanged";
    public const string TenantUpdated = "TenantUpdated";
    public const string SupportSessionStarted = "SupportSessionStarted";
    public const string SupportSessionEnded = "SupportSessionEnded";
    public const string SubscriptionChanged = "SubscriptionChanged";
    public const string AdminReportViewed = "AdminReportViewed";
}
