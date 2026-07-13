namespace Tripflow.UnitTests.Utils;

public class TestEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateOnly BirthDate { get; set; }
    public NestedTestEntity? Nested { get; set; }
}

public class NestedTestEntity
{
    public string Name { get; set; } = string.Empty;
}

public class EmptyEntity;

public class OnlyNameEntity
{
    public string Name { get; set; } = string.Empty;
}
