namespace Tripflow.Application.DTOs.Requests.Roles;

public sealed record RemoveUserRoleValidationRequest(Guid UserId, Guid RoleId);
