using Tripflow.Domain.Enums;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.DomainEntities;

public class CustomerTests
{
    [Fact]
    public void Constructor_Should_StartAsActive()
    {
        var customer = CustomerTestHelper.Create();

        Assert.Equal(CustomerStatus.Active, customer.Status);
        Assert.True(customer.CanReceiveQuotes());
    }

    [Fact]
    public void Update_Should_ChangeFields_AndSetUpdated()
    {
        var customer = CustomerTestHelper.Create();

        customer.Update(
            CustomerType.Company,
            "Nova Razão",
            "99988877766",
            "novo@email.com",
            "11000000000",
            new DateOnly(1980, 5, 10),
            "Novas notas.",
            CustomerStatus.Inactive,
            "user@test.com");

        Assert.Equal(CustomerType.Company, customer.Type);
        Assert.Equal("Nova Razão", customer.FullName);
        Assert.Equal("user@test.com", customer.UpdatedBy);
        Assert.NotNull(customer.UpdatedAtUtc);
        Assert.Equal(CustomerStatus.Inactive, customer.Status);
    }

    [Fact]
    public void Activate_Should_SetStatusActive()
    {
        var customer = CustomerTestHelper.Create();
        customer.Block("user@test.com");

        customer.Activate("user@test.com");

        Assert.Equal(CustomerStatus.Active, customer.Status);
        Assert.True(customer.CanReceiveQuotes());
    }

    [Fact]
    public void Inactivate_Should_SetStatusInactive()
    {
        var customer = CustomerTestHelper.Create();

        customer.Inactivate("user@test.com");

        Assert.Equal(CustomerStatus.Inactive, customer.Status);
        Assert.False(customer.CanReceiveQuotes());
    }

    [Fact]
    public void Block_Should_SetStatusBlocked()
    {
        var customer = CustomerTestHelper.Create();

        customer.Block("user@test.com");

        Assert.Equal(CustomerStatus.Blocked, customer.Status);
        Assert.False(customer.CanReceiveQuotes());
    }

    [Fact]
    public void CanReceiveQuotes_Should_Return_True_Only_For_Active()
    {
        var customer = CustomerTestHelper.Create();
        Assert.True(customer.CanReceiveQuotes());

        customer.Block("u");
        Assert.False(customer.CanReceiveQuotes());

        customer.Inactivate("u");
        Assert.False(customer.CanReceiveQuotes());

        customer.Activate("u");
        Assert.True(customer.CanReceiveQuotes());
    }
}
