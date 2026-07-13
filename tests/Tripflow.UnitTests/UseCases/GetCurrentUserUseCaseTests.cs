using Tripflow.UnitTests.Fixtures.UseCases;

namespace Tripflow.UnitTests.UseCases;

public class GetCurrentUserUseCaseTests(GetCurrentUserUseCaseFixture fixture) : IClassFixture<GetCurrentUserUseCaseFixture>
{
    [Fact]
    public async Task ExecuteAsync_Should_Return_Ok_With_Mapped_Response()
    {
        var useCase = fixture.CreateForSuccess();

        var result = await useCase.ExecuteAsync();

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("keycloak-user-123", result.Data!.IdentityProviderUserId);
        Assert.Equal("TripFlow", result.Data.TenantTradeName);
        Assert.Contains("users.read", result.Data.Permissions);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Forbidden_When_Not_Authenticated()
    {
        var useCase = fixture.CreateForUnauthenticated();

        var result = await useCase.ExecuteAsync();

        Assert.False(result.Success);
        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Identity_Is_Missing()
    {
        var useCase = fixture.CreateForMissingIdentityId();

        var result = await useCase.ExecuteAsync();

        Assert.False(result.Success);
        Assert.Equal("Usuário autenticado sem identificador válido.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Profile_Not_Found()
    {
        var useCase = fixture.CreateForProfileNotFound();

        var result = await useCase.ExecuteAsync();

        Assert.False(result.Success);
        Assert.Equal("Perfil de usuário não encontrado.", result.Error);
    }
}
