namespace Tripflow.Domain.Interfaces.Contexts;

public interface IUserContext
{
    string? IdentityProviderUserId { get; }
    string? Email { get; }
    string? Name { get; }
    IReadOnlyList<string> Roles { get; }
    bool IsAuthenticated { get; }
}
