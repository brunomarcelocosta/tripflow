using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.DomainEntities;

public class CustomerPreferenceTests
{
    [Fact]
    public void Constructor_Should_SetTenantAndCustomer()
    {
        var tenantId = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        var pref = CustomerPreferenceTestHelper.Create(
            tenantId: tenantId,
            customerId: customerId,
            createdBy: "user");

        Assert.Equal(tenantId, pref.TenantId);
        Assert.Equal(customerId, pref.CustomerId);
        Assert.Equal("user", pref.CreatedBy);
    }

    [Fact]
    public void Update_Should_ChangeFields_AndSetUpdated()
    {
        var pref = CustomerPreferenceTestHelper.Create();

        pref.Update(
            "GOL",
            "Resort",
            "Corredor",
            "Sem glúten",
            "Cruzeiros",
            "Cliente premium",
            "user@test.com");

        Assert.Equal("GOL", pref.PreferredAirlines);
        Assert.Equal("Resort", pref.PreferredHotelCategories);
        Assert.Equal("Corredor", pref.SeatPreferences);
        Assert.Equal("Sem glúten", pref.MealRestrictions);
        Assert.Equal("Cruzeiros", pref.TravelPreferences);
        Assert.Equal("Cliente premium", pref.GeneralNotes);
        Assert.Equal("user@test.com", pref.UpdatedBy);
        Assert.NotNull(pref.UpdatedAtUtc);
    }
}
