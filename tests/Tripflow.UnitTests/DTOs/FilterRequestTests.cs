using Tripflow.Application.DTOs.Requests;

namespace Tripflow.UnitTests.DTOs;

public class FilterRequestTests
{
    [Fact]
    public void Properties_Should_Be_Set_And_Read()
    {
        var request = new FilterRequest
        {
            Page = 2,
            PageSize = 25,
            SortBy = "LegalName",
            SortDesc = false,
            Search = "busca",
            Code = "COD",
            Name = "Nome"
        };

        Assert.Equal(2, request.Page);
        Assert.Equal(25, request.PageSize);
        Assert.Equal("LegalName", request.SortBy);
        Assert.False(request.SortDesc);
        Assert.Equal("busca", request.Search);
        Assert.Equal("COD", request.Code);
        Assert.Equal("Nome", request.Name);
    }
}
