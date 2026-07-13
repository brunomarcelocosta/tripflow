using Moq;
using Tripflow.Application.DTOs.Requests.Users;
using Tripflow.UnitTests.Fixtures.UseCases;

namespace Tripflow.UnitTests.UseCases;

public class InviteUserUseCaseTests(InviteUserUseCaseFixture fixture) : IClassFixture<InviteUserUseCaseFixture>
{
    private static InviteUserRequest ValidRequest() => new()
    {
        FullName = "Novo Usuário",
        Email = "novo@test.com",
        RoleNames = ["Consultant"],
        SendInviteEmail = false
    };

    [Fact]
    public async Task ExecuteAsync_Should_Return_Ok_When_Valid_Request()
    {
        var useCase = fixture.CreateForSuccess();

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("novo@test.com", result.Data!.Email);
        Assert.Contains("Consultant", result.Data.Roles);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Forbidden_Without_Permission()
    {
        var useCase = fixture.CreateForForbidden();

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Validation_Fails()
    {
        var useCase = fixture.CreateForValidationFailure("E-mail inválido.");

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("E-mail inválido.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Keycloak_Fails()
    {
        var useCase = fixture.CreateForKeycloakFailure();

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("Não foi possível criar o usuário no Keycloak.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_Rollback_When_Repository_Throws()
    {
        var useCase = fixture.CreateForRepositoryError();

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("Erro inesperado ao convidar usuário.", result.Error);
        fixture.MockUserProfileRepository.Verify(
            x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_Commit_When_Success()
    {
        var useCase = fixture.CreateForSuccess();

        await useCase.ExecuteAsync(ValidRequest());

        fixture.MockUserProfileRepository.Verify(
            x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
