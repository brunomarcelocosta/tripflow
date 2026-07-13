using Tripflow.Application.DTOs.Common;

namespace Tripflow.UnitTests.Common;

public class ResultTests
{
    [Fact]
    public void Ok_Should_Create_Successful_Result()
    {
        var result = Result<string>.Ok("data");

        Assert.True(result.Success);
        Assert.Equal("data", result.Data);
        Assert.Null(result.Error);
        Assert.False(result.IsForbidden);
    }

    [Fact]
    public void Failure_With_Message_Should_Create_Failed_Result()
    {
        var result = Result<string>.Failure("erro");

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("erro", result.Error);
        Assert.False(result.IsForbidden);
    }

    [Fact]
    public void Failure_With_Errors_Should_Join_Messages()
    {
        var result = Result<string>.Failure(["erro 1", "erro 2"]);

        Assert.False(result.Success);
        Assert.Equal("erro 1; erro 2", result.Error);
    }

    [Fact]
    public void Forbidden_Should_Create_Forbidden_Result()
    {
        var result = Result<string>.Forbidden();

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Acesso negado.", result.Error);
        Assert.True(result.IsForbidden);
    }

    [Fact]
    public void Forbidden_With_Custom_Message_Should_Use_Provided_Message()
    {
        var result = Result<string>.Forbidden("Sem permissão.");

        Assert.False(result.Success);
        Assert.Equal("Sem permissão.", result.Error);
        Assert.True(result.IsForbidden);
    }
}
