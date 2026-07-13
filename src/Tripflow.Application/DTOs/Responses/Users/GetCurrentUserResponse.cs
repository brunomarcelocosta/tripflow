using Tripflow.Domain.Enums;
using Tripflow.Application.DTOs.Responses.Subscriptions;

namespace Tripflow.Application.DTOs.Responses.Users;

public sealed record GetCurrentUserResponse
{
    public Guid UserProfileId { get; init; }
    public string IdentityProviderUserId { get; init; } = default!;
    public Guid TenantId { get; init; }
    public string TenantTradeName { get; init; } = default!;
    public string FullName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public UserStatus Status { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = [];
    public IReadOnlyList<string> Permissions { get; init; } = [];
    public TenantEntitlementsResponse? Entitlements { get; init; }
}
