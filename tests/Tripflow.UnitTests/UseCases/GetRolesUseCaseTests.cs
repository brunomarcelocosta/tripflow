using Tripflow.UnitTests.Fixtures.UseCases;

namespace Tripflow.UnitTests.UseCases;

public class GetRolesUseCaseTests(GetRolesUseCaseFixture fixture) : IClassFixture<GetRolesUseCaseFixture>
{
    [Fact]
    public async Task ExecuteAsync_Should_Return_Ok_With_Mapped_Roles()
    {
        var useCase = fixture.CreateForSuccess();

        var result = await useCase.ExecuteAsync();

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.Count);
        Assert.Contains(result.Data, x => x.Name == "Consultant" && x.Permissions.Contains("users.read"));
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Forbidden_Without_Permission()
    {
        var useCase = fixture.CreateForForbidden();

        var result = await useCase.ExecuteAsync();

        Assert.False(result.Success);
        Assert.True(result.IsForbidden);
    }
}
