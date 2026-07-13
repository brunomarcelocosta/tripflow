using Tripflow.Application.Helpers;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Helpers;

public class TravelerOrderByHelperTests
{
    [Theory]
    [InlineData("fullname")]
    [InlineData("nationality")]
    [InlineData("documentnumber")]
    [InlineData("passportnumber")]
    [InlineData("passportexpirationdate")]
    [InlineData("createdatutc")]
    [InlineData("updatedatutc")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("unknown")]
    public void Build_Should_Return_Compilable_Expression(string? sortBy)
    {
        var expression = TravelerOrderByHelper.Build(sortBy);
        var compiled = expression.Compile();
        var entity = TravelerTestHelper.Create();

        _ = compiled(entity);
    }

    [Fact]
    public void Build_WithFullName_OrdersByFullName()
    {
        var compiled = TravelerOrderByHelper.Build("FullName").Compile();
        var entity = TravelerTestHelper.Create(fullName: "ANA");
        Assert.Equal("ANA", compiled(entity));
    }

    [Fact]
    public void Build_WithPassportExpirationDate_OrdersByPassportExpirationDate()
    {
        var date = new DateOnly(2030, 12, 31);
        var compiled = TravelerOrderByHelper.Build("passportexpirationdate").Compile();
        var entity = TravelerTestHelper.Create(passportExpirationDate: date);
        Assert.Equal(date, compiled(entity));
    }
}
