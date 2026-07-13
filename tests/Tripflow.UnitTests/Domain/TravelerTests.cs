using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.DomainEntities;

public class TravelerTests
{
    [Fact]
    public void Update_Should_ChangeFields_AndSetUpdated()
    {
        var traveler = TravelerTestHelper.Create();

        traveler.Update(
            "Outro Nome",
            new DateOnly(2000, 1, 1),
            "Portuguesa",
            "12345",
            "PT999",
            new DateOnly(2035, 1, 1),
            "Outras notas.",
            "user@test.com");

        Assert.Equal("Outro Nome", traveler.FullName);
        Assert.Equal("PT999", traveler.PassportNumber);
        Assert.Equal("user@test.com", traveler.UpdatedBy);
        Assert.NotNull(traveler.UpdatedAtUtc);
    }

    [Fact]
    public void IsPassportExpired_Should_Be_True_When_Past()
    {
        var traveler = TravelerTestHelper.Create(
            passportExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1));

        Assert.True(traveler.IsPassportExpired());
        Assert.False(traveler.IsPassportExpiringSoon());
    }

    [Fact]
    public void IsPassportExpired_Should_Be_False_When_NoDate()
    {
        var traveler = TravelerTestHelper.Create(passportExpirationDate: null);

        Assert.False(traveler.IsPassportExpired());
        Assert.False(traveler.IsPassportExpiringSoon());
    }

    [Fact]
    public void IsPassportExpiringSoon_Should_Be_True_When_Within_6_Months()
    {
        var traveler = TravelerTestHelper.Create(
            passportExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(3));

        Assert.False(traveler.IsPassportExpired());
        Assert.True(traveler.IsPassportExpiringSoon());
    }

    [Fact]
    public void IsPassportExpiringSoon_Should_Be_False_When_Far_Future()
    {
        var traveler = TravelerTestHelper.Create(
            passportExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow).AddYears(5));

        Assert.False(traveler.IsPassportExpired());
        Assert.False(traveler.IsPassportExpiringSoon());
    }
}
