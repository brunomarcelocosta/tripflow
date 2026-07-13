using Moq;
using Tripflow.Application.Validators.Users;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.UnitTests.Fixtures.Validators;

public class InviteUserRequestValidatorFixture : IDisposable
{
    public Mock<ITenantContext> MockTenantContext { get; private set; } = null!;
    public Mock<IUserProfileRepository> MockUserProfileRepository { get; private set; } = null!;
    public InviteUserRequestValidator Validator { get; private set; } = null!;
    public Guid TenantId { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    public InviteUserRequestValidatorFixture()
    {
        Reset();
    }

    public InviteUserRequestValidator CreateValidatorWithoutDuplicateEmail()
    {
        Reset();
        MockUserProfileRepository
            .Setup(x => x.ExistsByEmailInTenantAsync(TenantId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return Validator;
    }

    public InviteUserRequestValidator CreateValidatorWithDuplicateEmail()
    {
        Reset();
        MockUserProfileRepository
            .Setup(x => x.ExistsByEmailInTenantAsync(TenantId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        return Validator;
    }

    private void Reset()
    {
        MockTenantContext = new Mock<ITenantContext>();
        MockUserProfileRepository = new Mock<IUserProfileRepository>();
        MockTenantContext.Setup(x => x.HasTenant).Returns(true);
        MockTenantContext.Setup(x => x.TenantId).Returns(TenantId);
        Validator = new InviteUserRequestValidator(MockTenantContext.Object, MockUserProfileRepository.Object);
    }

    public void Dispose() { }
}
