using Tripflow.Domain.Enums;

namespace Tripflow.Domain.Entities.Miles;

public sealed class LoyaltyProgram : AuditableEntity
{
    private LoyaltyProgram() { }

    public LoyaltyProgram(string name, string? country, string? airlineName, LoyaltyProgramStatus status, string createdBy)
    {
        Name = name;
        Country = country;
        AirlineName = airlineName;
        Status = status;
        SetCreated(createdBy);
    }

    public string Name { get; private set; } = default!;
    public string? Country { get; private set; }
    public string? AirlineName { get; private set; }
    public LoyaltyProgramStatus Status { get; private set; }

    public void Update(string name, string? country, string? airlineName, LoyaltyProgramStatus status, string updatedBy)
    {
        Name = name;
        Country = country;
        AirlineName = airlineName;
        Status = status;
        SetUpdated(updatedBy);
    }

    public void Activate(string updatedBy)
    {
        Status = LoyaltyProgramStatus.Active;
        SetUpdated(updatedBy);
    }

    public void Inactivate(string updatedBy)
    {
        Status = LoyaltyProgramStatus.Inactive;
        SetUpdated(updatedBy);
    }
}
