using Moq;
using Tripflow.UnitTests.Fixtures.UseCases;

namespace Tripflow.UnitTests.UseCases;

public class RemoveUserRoleUseCaseTests(RemoveUserRoleUseCaseFixture fixture) : IClassFixture<RemoveUserRoleUseCaseFixture>
{
    [Fact]
    public async Task ExecuteAsync_Should_Return_Ok_When_Success()
    {
        var useCase = fixture.CreateForSuccess();

        var result = await useCase.ExecuteAsync(fixture.UserId, fixture.RoleId);

        Assert.True(result.Success);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Validation_Fails()
    {
        var useCase = fixture.CreateForValidationFailure("Role não encontrada nesta empresa.");

        var result = await useCase.ExecuteAsync(fixture.UserId, fixture.RoleId);

        Assert.False(result.Success);
        Assert.Equal("Role não encontrada nesta empresa.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_Rollback_When_Repository_Throws()
    {
        var useCase = fixture.CreateForRepositoryError();

        var result = await useCase.ExecuteAsync(fixture.UserId, fixture.RoleId);

        Assert.False(result.Success);
        Assert.Equal("Erro inesperado ao remover role do usuário.", result.Error);
        fixture.MockUserProfileRepository.Verify(
            x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
