using AutoMapper;
using Moq;
using Tripflow.Application.UseCases.Users;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Fixtures.UseCases;

public class GetCurrentUserUseCaseFixture : IDisposable
{
    public Mock<IUserContext> MockUserContext { get; private set; } = null!;
    public Mock<ITenantContext> MockTenantContext { get; private set; } = null!;
    public Mock<IUserProfileRepository> MockUserProfileRepository { get; private set; } = null!;
    public Mock<ITenantSubscriptionRepository> MockTenantSubscriptionRepository { get; private set; } = null!;
    public IMapper Mapper { get; }
    public GetCurrentUserUseCase UseCase { get; private set; } = null!;

    public GetCurrentUserUseCaseFixture()
    {
        Mapper = AutoMapperTestHelper.CreateMapper();
        ResetMocks();
    }

    private void ResetMocks()
    {
        MockUserContext = new Mock<IUserContext>();
        MockTenantContext = new Mock<ITenantContext>();
        MockUserProfileRepository = new Mock<IUserProfileRepository>();
        MockTenantSubscriptionRepository = new Mock<ITenantSubscriptionRepository>();
        UseCase = new GetCurrentUserUseCase(
            MockUserContext.Object,
            MockTenantContext.Object,
            MockUserProfileRepository.Object,
            MockTenantSubscriptionRepository.Object,
            Mapper);
    }

    public GetCurrentUserUseCase CreateForSuccess(UserProfile? profile = null)
    {
        ResetMocks();
        MockUserContext.Setup(x => x.IsAuthenticated).Returns(true);
        MockUserContext.Setup(x => x.IdentityProviderUserId).Returns("keycloak-user-123");
        MockTenantContext.Setup(x => x.HasTenant).Returns(true);
        MockTenantContext.Setup(x => x.TenantId).Returns(Guid.Parse("22222222-2222-2222-2222-222222222221"));
        MockTenantSubscriptionRepository
            .Setup(x => x.GetByTenantIdWithPlanFeaturesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tripflow.Domain.Entities.Subscriptions.TenantSubscription?)null);

        var userProfile = profile ?? CreateDefaultProfile();
        MockUserProfileRepository
            .Setup(x => x.GetByIdentityProviderUserIdAsync("keycloak-user-123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(userProfile);

        return UseCase;
    }

    public GetCurrentUserUseCase CreateForUnauthenticated()
    {
        ResetMocks();
        MockUserContext.Setup(x => x.IsAuthenticated).Returns(false);
        return UseCase;
    }

    public GetCurrentUserUseCase CreateForMissingIdentityId()
    {
        ResetMocks();
        MockUserContext.Setup(x => x.IsAuthenticated).Returns(true);
        MockUserContext.Setup(x => x.IdentityProviderUserId).Returns((string?)null);
        return UseCase;
    }

    public GetCurrentUserUseCase CreateForProfileNotFound()
    {
        ResetMocks();
        MockUserContext.Setup(x => x.IsAuthenticated).Returns(true);
        MockUserContext.Setup(x => x.IdentityProviderUserId).Returns("keycloak-user-123");
        MockUserProfileRepository
            .Setup(x => x.GetByIdentityProviderUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);
        return UseCase;
    }

    private static UserProfile CreateDefaultProfile()
    {
        var tenant = TenantTestHelper.Create(tradeName: "TripFlow");
        var permission = IdentityTestHelper.CreatePermission(Tripflow.Infra.Data.Seeds.TripflowDbSeedData.Permissions.UsersRead);
        var role = IdentityTestHelper.CreateRole(tenant.Id, permissions: permission);
        var profile = IdentityTestHelper.CreateUserProfile(tenant: tenant);
        IdentityTestHelper.AddRoleToUser(profile, role);
        return profile;
    }

    public void Dispose() { }
}
