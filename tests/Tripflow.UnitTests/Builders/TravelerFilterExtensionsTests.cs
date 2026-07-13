using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Requests.Travelers;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Builders;

public class TravelerFilterExtensionsTests
{
    [Fact]
    public void ToExpression_NoFilters_AllowAll()
    {
        var request = new TravelerFilterRequest();
        var traveler = TravelerTestHelper.Create();

        Assert.True(request.ToExpression().Compile()(traveler));
    }

    [Fact]
    public void ToExpression_WithCustomerId_FiltersByCustomer()
    {
        var customerId = Guid.NewGuid();
        var request = new TravelerFilterRequest();

        var matching = TravelerTestHelper.Create(customerId: customerId);
        var other = TravelerTestHelper.Create();

        var compiled = request.ToExpression(customerId).Compile();
        Assert.True(compiled(matching));
        Assert.False(compiled(other));
    }

    [Fact]
    public void ToExpression_WithSearch_MatchesByFullName()
    {
        var request = new TravelerFilterRequest { Search = "MARIA" };
        var matching = TravelerTestHelper.Create(fullName: "Maria Silva");

        Assert.True(request.ToExpression().Compile()(matching));
    }

    [Fact]
    public void ToExpression_WithPassportExpiringBefore_FiltersByDate()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1);
        var request = new TravelerFilterRequest { PassportExpiringBefore = date };

        var expires = TravelerTestHelper.Create(passportExpirationDate: date.AddMonths(-1));
        var validFuture = TravelerTestHelper.Create(passportExpirationDate: date.AddYears(5));

        var compiled = request.ToExpression().Compile();
        Assert.True(compiled(expires));
        Assert.False(compiled(validFuture));
    }

    [Fact]
    public void ToExpression_WithPassportNumber_FiltersByPassport()
    {
        var request = new TravelerFilterRequest { PassportNumber = "BR999" };
        var match = TravelerTestHelper.Create(passportNumber: "BR999");
        var noMatch = TravelerTestHelper.Create(passportNumber: "BR111");

        var compiled = request.ToExpression().Compile();
        Assert.True(compiled(match));
        Assert.False(compiled(noMatch));
    }

    [Fact]
    public void ToExpression_WithNationality_FiltersByNationality()
    {
        var request = new TravelerFilterRequest { Nationality = "brasileira" };
        var match = TravelerTestHelper.Create(nationality: "Brasileira");
        var noMatch = TravelerTestHelper.Create(nationality: "Portuguesa");

        var compiled = request.ToExpression().Compile();
        Assert.True(compiled(match));
        Assert.False(compiled(noMatch));
    }
}
