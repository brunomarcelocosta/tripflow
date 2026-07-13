using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tripflow.Application.UseCases.Customers;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.UseCases;

public class CustomerStatusUseCasesTests
{
    private static (Mock<ICustomerRepository> repo, Mock<ITravelerRepository> trav, Mock<IUserContext> user, Mock<ITenantContext> tenant, Mock<IUserPermissionService> perm, Guid tenantId) CreateMocks(Customer? trackedCustomer)
    {
        var tenantId = trackedCustomer?.TenantId ?? Guid.NewGuid();
        var repo = new Mock<ICustomerRepository>();
        var trav = new Mock<ITravelerRepository>();
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
            .ReturnsAsync(trackedCustomer);
        trav.Setup(x => x.CountByCustomersAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, int>());
        return (repo, trav, user, tenant, perm, tenantId);
    }

    [Fact]
    public async Task ActivateCustomer_Should_Set_Status_Active()
    {
        var customer = CustomerTestHelper.Create();
        customer.Block("system");

        var (repo, trav, user, tenant, perm, _) = CreateMocks(customer);
        var useCase = new ActivateCustomerUseCase(repo.Object, trav.Object, user.Object, tenant.Object, perm.Object, NullLogger<ActivateCustomerUseCase>.Instance);

        var result = await useCase.ExecuteAsync(customer.Id);

        Assert.True(result.Success);
        Assert.Equal(CustomerStatus.Active, result.Data!.Status);
    }

    [Fact]
    public async Task InactivateCustomer_Should_Set_Status_Inactive()
    {
        var customer = CustomerTestHelper.Create();

        var (repo, trav, user, tenant, perm, _) = CreateMocks(customer);
        var useCase = new InactivateCustomerUseCase(repo.Object, trav.Object, user.Object, tenant.Object, perm.Object, NullLogger<InactivateCustomerUseCase>.Instance);

        var result = await useCase.ExecuteAsync(customer.Id);

        Assert.True(result.Success);
        Assert.Equal(CustomerStatus.Inactive, result.Data!.Status);
    }

    [Fact]
    public async Task BlockCustomer_Should_Set_Status_Blocked()
    {
        var customer = CustomerTestHelper.Create();

        var (repo, trav, user, tenant, perm, _) = CreateMocks(customer);
        var useCase = new BlockCustomerUseCase(repo.Object, trav.Object, user.Object, tenant.Object, perm.Object, NullLogger<BlockCustomerUseCase>.Instance);

        var result = await useCase.ExecuteAsync(customer.Id);

        Assert.True(result.Success);
        Assert.Equal(CustomerStatus.Blocked, result.Data!.Status);
    }

    [Fact]
    public async Task ActivateCustomer_Should_Return_Failure_When_NotFound()
    {
        var (repo, trav, user, tenant, perm, _) = CreateMocks(null);
        var useCase = new ActivateCustomerUseCase(repo.Object, trav.Object, user.Object, tenant.Object, perm.Object, NullLogger<ActivateCustomerUseCase>.Instance);

        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.False(result.Success);
    }
}
