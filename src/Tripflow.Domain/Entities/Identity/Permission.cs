namespace Tripflow.Domain.Entities.Identity;

public sealed class Permission : BaseEntity
{
    private Permission() { }

    public Permission(string code, string? description)
    {
        Code = code;
        Description = description;
    }

    public string Code { get; private set; } = default!;
    public string? Description { get; private set; }

    public List<RolePermission> RolePermissions = [];
}

