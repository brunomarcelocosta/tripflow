using Moq;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Domain.Common;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Enums;
using Tripflow.UnitTests.Fixtures.UseCases;

namespace Tripflow.UnitTests.UseCases;

public class AdminUserUseCaseTests
{
    [Fact]
    public async Task GetAdminUsersUseCase_Should_Return_Forbidden_When_Not_Authenticated()
    {
        var fixture = new AdminUserUseCaseFixture();
        fixture.MockUserContext.Setup(x => x.IsAuthenticated).Returns(false);
        var useCase = fixture.CreateGetAdminUsersUseCase();

        var result = await useCase.ExecuteAsync(new AdminUserFilterRequest { Page = 1, PageSize = 10 });

        Assert.False(result.Success);
        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task GetAdminUsersUseCase_Should_Return_Paged_Users()
    {
        var fixture = new AdminUserUseCaseFixture();
        var user = fixture.CreateUser();

        fixture.MockUserProfileRepository
            .Setup(x => x.GetPagedForAdminAsync(
                null,
                It.IsAny<System.Linq.Expressions.Expression<Func<UserProfile, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<UserProfile, object>>?>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<UserProfile>
            {
                Items = [user],
                Page = 1,
                PageSize = 10,
                TotalItems = 1,
                TotalPages = 1
            });

        var useCase = fixture.CreateGetAdminUsersUseCase();
        var result = await useCase.ExecuteAsync(new AdminUserFilterRequest { Page = 1, PageSize = 10 });

        Assert.True(result.Success);
        Assert.Single(result.Data!.Items);
        Assert.Equal("user@test.com", result.Data.Items.First().Email);
    }

    [Fact]
    public async Task UpdateAdminUserUseCase_Should_Update_User_And_Keycloak()
    {
        var fixture = new AdminUserUseCaseFixture();
        var user = fixture.CreateUser();
        var request = new UpdateAdminUserRequest("Novo Nome", "user@test.com", "11888888888", UserStatus.Active);

        fixture.MockUserProfileRepository
            .Setup(x => x.GetTrackedByIdForAdminAsync(fixture.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        fixture.MockUserProfileRepository
            .Setup(x => x.GetByIdForAdminAsync(fixture.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        fixture.MockKeycloakUserService
            .Setup(x => x.UpdateUserAsync("keycloak-123", request.Email, request.FullName, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var useCase = fixture.CreateUpdateAdminUserUseCase();
        var result = await useCase.ExecuteAsync(fixture.UserId, request);

        Assert.True(result.Success);
        Assert.Equal("Novo Nome", result.Data!.FullName);
        fixture.MockKeycloakUserService.Verify(
            x => x.UpdateUserAsync("keycloak-123", request.Email, request.FullName, true, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetAdminUserPasswordUseCase_Should_Set_Password_In_Keycloak()
    {
        var fixture = new AdminUserUseCaseFixture();
        var user = fixture.CreateUser();

        fixture.MockUserProfileRepository
            .Setup(x => x.GetByIdForAdminAsync(fixture.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        fixture.MockKeycloakUserService
            .Setup(x => x.SetPasswordAsync("keycloak-123", "Senha@123", false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var useCase = fixture.CreateSetAdminUserPasswordUseCase();
        var result = await useCase.ExecuteAsync(fixture.UserId, new SetAdminUserPasswordRequest("Senha@123"));

        Assert.True(result.Success);
        Assert.True(result.Data);
    }
}
