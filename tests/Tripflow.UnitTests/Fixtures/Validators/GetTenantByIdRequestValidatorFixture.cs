using Moq;
using Tripflow.Application.Validators.Tenants;
using Tripflow.Domain.Interfaces;

namespace Tripflow.UnitTests.Fixtures.Validators;

public class GetTenantByIdRequestValidatorFixture : IDisposable
{
    public Mock<ITenantRepository> MockRepository { get; private set; } = null!;
    public GetTenantByIdRequestValidator Validator { get; private set; } = null!;

    public GetTenantByIdRequestValidatorFixture()
    {
        MockRepository = new Mock<ITenantRepository>();
        Validator = new GetTenantByIdRequestValidator(MockRepository.Object);
    }

    public GetTenantByIdRequestValidator CreateForExists()
    {
        MockRepository
            .Setup(repo => repo.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Tripflow.Domain.Entities.Tenants.Tenant, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        return Validator;
    }

    public GetTenantByIdRequestValidator CreateForNotExists()
    {
        MockRepository
            .Setup(repo => repo.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Tripflow.Domain.Entities.Tenants.Tenant, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return Validator;
    }

    public void Dispose() { }
}
