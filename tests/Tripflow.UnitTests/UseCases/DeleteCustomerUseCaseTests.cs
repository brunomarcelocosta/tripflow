using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tripflow.Application.UseCases.Customers;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.UseCases;

public class DeleteCustomerUseCaseTests
{
    private static (Mock<ICustomerRepository> repo, Mock<IUserContext> user, Mock<ITenantContext> tenant, Mock<IUserPermissionService> perm) CreateMocks(Guid tenantId, Customer? tracked)
    {
        var repo = new Mock<ICustomerRepository>();
        var user = new Mock<IUserContext>();
        var tenant = new Mock<ITenantContext>();
        var perm = new Mock<IUserPermissionService>();

        user.Setup(x => x.IsAuthenticated).Returns(true);
        user.Setup(x => x.Email).Returns("user@test.com");
        tenant.Setup(x => x.HasTenant).Returns(true);
        tenant.Setup(x => x.TenantId).Returns(tenantId);
        perm.Setup(x => x.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        repo.Setup(x => x.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        repo.Setup(x => x.GetTrackedByIdAndTenantAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tracked);
        return (repo, user, tenant, perm);
    }

    [Fact]
    public async Task ExecuteAsync_Should_SoftDelete_Customer()
    {
        var tenantId = Guid.NewGuid();
        var customer = CustomerTestHelper.Create(tenantId: tenantId);
        var (repo, user, tenant, perm) = CreateMocks(tenantId, customer);

        var useCase = new DeleteCustomerUseCase(repo.Object, user.Object, tenant.Object, perm.Object, NullLogger<DeleteCustomerUseCase>.Instance);
        var result = await useCase.ExecuteAsync(customer.Id);

        Assert.True(result.Success);
        Assert.True(customer.IsDeleted);
        Assert.Equal("user@test.com", customer.DeletedBy);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_NotFound()
    {
        var (repo, user, tenant, perm) = CreateMocks(Guid.NewGuid(), null);
        var useCase = new DeleteCustomerUseCase(repo.Object, user.Object, tenant.Object, perm.Object, NullLogger<DeleteCustomerUseCase>.Instance);

        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Equal("Cliente não encontrado.", result.Error);
    }
}
