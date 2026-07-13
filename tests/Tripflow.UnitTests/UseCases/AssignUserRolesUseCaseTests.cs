using Moq;
using Tripflow.Application.DTOs.Requests.Roles;
using Tripflow.UnitTests.Fixtures.UseCases;

namespace Tripflow.UnitTests.UseCases;

public class AssignUserRolesUseCaseTests(AssignUserRolesUseCaseFixture fixture) : IClassFixture<AssignUserRolesUseCaseFixture>
{
    private static AssignUserRolesRequest ValidRequest() => new()
    {
        RoleNames = ["Consultant"]
    };

    [Fact]
    public async Task ExecuteAsync_Should_Return_Ok_With_Assigned_Roles()
    {
        var useCase = fixture.CreateForSuccess();

        var result = await useCase.ExecuteAsync(fixture.UserId, ValidRequest());

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Contains("Consultant", result.Data!);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Validation_Fails()
    {
        var useCase = fixture.CreateForValidationFailure("Usuário não encontrado nesta empresa.");

        var result = await useCase.ExecuteAsync(fixture.UserId, ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("Usuário não encontrado nesta empresa.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_Rollback_When_Repository_Throws()
    {
        var useCase = fixture.CreateForRepositoryError();

        var result = await useCase.ExecuteAsync(fixture.UserId, ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("Erro inesperado ao atribuir roles ao usuário.", result.Error);
        fixture.MockUserProfileRepository.Verify(
            x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
