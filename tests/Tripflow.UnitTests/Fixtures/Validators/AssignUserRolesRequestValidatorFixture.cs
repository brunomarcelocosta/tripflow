using Moq;
using Tripflow.Application.Validators.Roles;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Fixtures.Validators;

public class AssignUserRolesRequestValidatorFixture : IDisposable
{
    public Mock<ITenantContext> MockTenantContext { get; private set; } = null!;
    public Mock<IUserProfileRepository> MockUserProfileRepository { get; private set; } = null!;
    public AssignUserRolesRequestValidator Validator { get; private set; } = null!;
    public Guid TenantId { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public Guid UserId { get; } = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    public AssignUserRolesRequestValidatorFixture()
    {
        Reset();
    }

    public AssignUserRolesRequestValidator CreateValidatorWithExistingUser()
    {
        Reset();
        var profile = IdentityTestHelper.CreateUserProfile(tenantId: TenantId);
        MockUserProfileRepository
            .Setup(x => x.GetByIdInTenantAsync(TenantId, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        return Validator;
    }

    public AssignUserRolesRequestValidator CreateValidatorWithMissingUser()
    {
        Reset();
        MockUserProfileRepository
            .Setup(x => x.GetByIdInTenantAsync(TenantId, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);
        return Validator;
    }

    private void Reset()
    {
        MockTenantContext = new Mock<ITenantContext>();
        MockUserProfileRepository = new Mock<IUserProfileRepository>();
        MockTenantContext.Setup(x => x.HasTenant).Returns(true);
        MockTenantContext.Setup(x => x.TenantId).Returns(TenantId);
        Validator = new AssignUserRolesRequestValidator(MockTenantContext.Object, MockUserProfileRepository.Object);
    }

    public void Dispose() { }
}
