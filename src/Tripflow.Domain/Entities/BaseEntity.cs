namespace Tripflow.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;
    public string CreatedBy { get; protected set; } = default!;

    public DateTime? UpdatedAtUtc { get; protected set; }
    public string? UpdatedBy { get; protected set; }

    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAtUtc { get; protected set; }
    public string? DeletedBy { get; protected set; }

    public void SetCreated(string user)
    {
        CreatedAtUtc = DateTime.UtcNow;
        CreatedBy = user;
    }

    public void SetUpdated(string user)
    {
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = user;
    }

    public void SetDelete(string user)
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        DeletedBy = user;
    }
}
