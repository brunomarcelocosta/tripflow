using Moq;
using Tripflow.Application.Validators.Tenants;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.UnitTests.Fixtures.Validators;

public class CreateTenantRequestValidatorFixture : IDisposable
{
    public Mock<ITenantRepository> MockRepository { get; private set; } = null!;
    public CreateTenantRequestValidator Validator { get; private set; } = null!;

    public CreateTenantRequestValidatorFixture()
    {
        MockRepository = new Mock<ITenantRepository>();
        Validator = new CreateTenantRequestValidator(MockRepository.Object);
    }

    public CreateTenantRequestValidator CreateValidatorForNoDuplicates()
    {
        MockRepository
            .Setup(repo => repo.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Tenant, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return Validator;
    }

    public CreateTenantRequestValidator CreateValidatorForDocumentExists()
    {
        MockRepository
            .SetupSequence(repo => repo.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Tenant, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        return Validator;
    }

    public CreateTenantRequestValidator CreateValidatorForEmailExists()
    {
        MockRepository
            .SetupSequence(repo => repo.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Tenant, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false)
            .ReturnsAsync(true);
        return Validator;
    }

    public void Dispose() { }
}
