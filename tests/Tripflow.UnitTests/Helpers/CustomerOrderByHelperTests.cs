using Tripflow.Application.Helpers;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Helpers;

public class CustomerOrderByHelperTests
{
    [Theory]
    [InlineData("fullname")]
    [InlineData("email")]
    [InlineData("phone")]
    [InlineData("documentnumber")]
    [InlineData("type")]
    [InlineData("status")]
    [InlineData("createdatutc")]
    [InlineData("updatedatutc")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("unknown")]
    public void Build_Should_Return_Compilable_Expression(string? sortBy)
    {
        var expression = CustomerOrderByHelper.Build(sortBy);
        var compiled = expression.Compile();
        var entity = CustomerTestHelper.Create();

        _ = compiled(entity);
    }

    [Fact]
    public void Build_WithFullName_OrdersByFullName()
    {
        var compiled = CustomerOrderByHelper.Build("FullName").Compile();
        var entity = CustomerTestHelper.Create(fullName: "ZULMA");
        Assert.Equal("ZULMA", compiled(entity));
    }

    [Fact]
    public void Build_WithUnknown_FallbackToCreatedAtUtc()
    {
        var compiled = CustomerOrderByHelper.Build("xyz").Compile();
        var entity = CustomerTestHelper.Create();
        Assert.IsType<DateTime>(compiled(entity));
    }
}
