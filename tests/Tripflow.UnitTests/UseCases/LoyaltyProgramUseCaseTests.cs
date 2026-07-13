using FluentValidation;
using FluentValidation.Results;
using Moq;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.UseCases.Miles;
using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.UnitTests.UseCases;

public class LoyaltyProgramUseCaseTests
{
    [Fact]
    public async Task CreateLoyaltyProgramUseCase_Should_RejectDuplicateName()
    {
        var repository = new Mock<ILoyaltyProgramRepository>();
        repository.Setup(r => r.ExistsByNameAsync("Smiles", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var validator = new Mock<IValidator<CreateLoyaltyProgramRequest>>();
        validator.Setup(v => v.ValidateAsync(It.IsAny<CreateLoyaltyProgramRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var user = new Mock<IUserContext>();
        user.Setup(x => x.IsAuthenticated).Returns(true);
        user.Setup(x => x.Email).Returns("admin@test.com");

        var perm = new Mock<IUserPermissionService>();
        perm.Setup(p => p.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var useCase = new CreateLoyaltyProgramUseCase(repository.Object, validator.Object, user.Object, perm.Object);

        var result = await useCase.ExecuteAsync(new CreateLoyaltyProgramRequest("Smiles", "BR", "GOL", null));

        Assert.False(result.Success);
        Assert.Contains("nome", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteLoyaltyProgramUseCase_Should_BlockWhenLinkedAccountsExist()
    {
        var program = new LoyaltyProgram("Smiles", "BR", "GOL", LoyaltyProgramStatus.Active, "system");
        var programId = program.Id;

        var repository = new Mock<ILoyaltyProgramRepository>();
        repository.Setup(r => r.GetTrackedByIdAsync(programId, It.IsAny<CancellationToken>())).ReturnsAsync(program);
        repository.Setup(r => r.HasLinkedAccountsAsync(programId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var user = new Mock<IUserContext>();
        user.Setup(x => x.IsAuthenticated).Returns(true);
        user.Setup(x => x.Email).Returns("admin@test.com");

        var perm = new Mock<IUserPermissionService>();
        perm.Setup(p => p.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var useCase = new DeleteLoyaltyProgramUseCase(repository.Object, user.Object, perm.Object);
        var result = await useCase.ExecuteAsync(programId);

        Assert.False(result.Success);
        repository.Verify(r => r.UpdateAsync(It.IsAny<LoyaltyProgram>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
