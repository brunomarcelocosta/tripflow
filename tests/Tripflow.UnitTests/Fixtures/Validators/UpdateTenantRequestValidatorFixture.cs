using Moq;
using Tripflow.Application.Validators.Tenants;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.UnitTests.Fixtures.Validators;

public class UpdateTenantRequestValidatorFixture : IDisposable
{
    public Mock<ITenantRepository> MockRepository { get; private set; } = null!;
    public UpdateTenantRequestValidator Validator { get; private set; } = null!;

    public UpdateTenantRequestValidatorFixture()
    {
        MockRepository = new Mock<ITenantRepository>();
        Validator = new UpdateTenantRequestValidator(MockRepository.Object);
    }

    public UpdateTenantRequestValidator CreateForSuccess()
    {
        MockRepository
            .SetupSequence(repo => repo.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Tenant, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false)
            .ReturnsAsync(false);
        return Validator;
    }

    public UpdateTenantRequestValidator CreateForIdNotExists()
    {
        MockRepository
            .Setup(repo => repo.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Tenant, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return Validator;
    }

    public UpdateTenantRequestValidator CreateForDocumentExistsInOther()
    {
        MockRepository
            .SetupSequence(repo => repo.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Tenant, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        return Validator;
    }

    public UpdateTenantRequestValidator CreateForEmailExistsInOther()
    {
        MockRepository
            .SetupSequence(repo => repo.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Tenant, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false)
            .ReturnsAsync(true);
        return Validator;
    }

    public void Dispose() { }
}
