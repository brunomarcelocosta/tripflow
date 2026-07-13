namespace Tripflow.Application.DTOs.Requests.Roles;

public sealed record AssignUserRolesValidationRequest(
    Guid UserId,
    List<Guid>? RoleIds,
    List<string>? RoleNames)
{
    public static AssignUserRolesValidationRequest From(Guid userId, AssignUserRolesRequest request) =>
        new(userId, request.RoleIds, request.RoleNames);
}
