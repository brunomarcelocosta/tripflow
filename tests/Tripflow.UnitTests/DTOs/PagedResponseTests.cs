using Tripflow.Application.DTOs.Responses;

namespace Tripflow.UnitTests.DTOs;

public class PagedResponseTests
{
    [Fact]
    public void ParameterlessConstructor_Should_Initialize_Defaults()
    {
        var response = new PagedResponse<string>();

        Assert.Empty(response.Items);
        Assert.Equal(0, response.Page);
        Assert.Equal(0, response.PageSize);
        Assert.Equal(0, response.TotalItems);
        Assert.Equal(0, response.TotalPages);
    }

    [Fact]
    public void Constructor_Should_Set_All_Properties()
    {
        var items = new[] { "a", "b" };
        var response = new PagedResponse<string>(items, 2, 10, 20, 2);

        Assert.Equal(items, response.Items);
        Assert.Equal(2, response.Page);
        Assert.Equal(10, response.PageSize);
        Assert.Equal(20, response.TotalItems);
        Assert.Equal(2, response.TotalPages);
    }
}
