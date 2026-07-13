using Tripflow.Domain.Enums;

namespace Tripflow.Application.DTOs.Requests.Admin;

public sealed record UpdateAdminUserRequest(
    string FullName,
    string Email,
    string? Phone,
    UserStatus Status);

public sealed record SetAdminUserPasswordRequest(
    string Password,
    bool Temporary = false);
