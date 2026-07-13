using Moq;
using Tripflow.Application.Validators.Roles;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Fixtures.Validators;

public class RemoveUserRoleRequestValidatorFixture : IDisposable
{
    public Mock<ITenantContext> MockTenantContext { get; private set; } = null!;
    public Mock<IUserProfileRepository> MockUserProfileRepository { get; private set; } = null!;
    public Mock<IRoleRepository> MockRoleRepository { get; private set; } = null!;
    public RemoveUserRoleRequestValidator Validator { get; private set; } = null!;
    public Guid TenantId { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public Guid UserId { get; } = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public Guid RoleId { get; } = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    public RemoveUserRoleRequestValidatorFixture()
    {
        Reset();
    }

    public RemoveUserRoleRequestValidator CreateValidatorWithValidEntities()
    {
        Reset();
        var profile = IdentityTestHelper.CreateUserProfile(tenantId: TenantId);
        var role = IdentityTestHelper.CreateRole(TenantId);
        typeof(Role).GetProperty(nameof(Role.Id))!.SetValue(role, RoleId);

        MockUserProfileRepository
            .Setup(x => x.GetByIdInTenantAsync(TenantId, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        MockRoleRepository.Setup(x => x.GetByIdAsync(RoleId)).ReturnsAsync(role);
        return Validator;
    }

    public RemoveUserRoleRequestValidator CreateValidatorWithMissingUser()
    {
        Reset();
        var role = IdentityTestHelper.CreateRole(TenantId);
        typeof(Role).GetProperty(nameof(Role.Id))!.SetValue(role, RoleId);
        MockRoleRepository.Setup(x => x.GetByIdAsync(RoleId)).ReturnsAsync(role);
        MockUserProfileRepository
            .Setup(x => x.GetByIdInTenantAsync(TenantId, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);
        return Validator;
    }

    public RemoveUserRoleRequestValidator CreateValidatorWithRoleFromOtherTenant()
    {
        Reset();
        var profile = IdentityTestHelper.CreateUserProfile(tenantId: TenantId);
        var role = IdentityTestHelper.CreateRole(Guid.NewGuid());
        typeof(Role).GetProperty(nameof(Role.Id))!.SetValue(role, RoleId);

        MockUserProfileRepository
            .Setup(x => x.GetByIdInTenantAsync(TenantId, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        MockRoleRepository.Setup(x => x.GetByIdAsync(RoleId)).ReturnsAsync(role);
        return Validator;
    }

    private void Reset()
    {
        MockTenantContext = new Mock<ITenantContext>();
        MockUserProfileRepository = new Mock<IUserProfileRepository>();
        MockRoleRepository = new Mock<IRoleRepository>();
        MockTenantContext.Setup(x => x.HasTenant).Returns(true);
        MockTenantContext.Setup(x => x.TenantId).Returns(TenantId);
        Validator = new RemoveUserRoleRequestValidator(
            MockTenantContext.Object,
            MockUserProfileRepository.Object,
            MockRoleRepository.Object);
    }

    public void Dispose() { }
}
