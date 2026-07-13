using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tripflow.Application.UseCases.Customers;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.UseCases;

public class GetCustomerPreferenceUseCaseTests
{
    private static GetCustomerPreferenceUseCase Build(Customer? customer, CustomerPreference? pref, out Guid tenantId)
    {
        tenantId = customer?.TenantId ?? Guid.NewGuid();
        var custRepo = new Mock<ICustomerRepository>();
        var prefRepo = new Mock<ICustomerPreferenceRepository>();
        var user = new Mock<IUserContext>();
        var tenant = new Mock<ITenantContext>();
        var perm = new Mock<IUserPermissionService>();

        user.Setup(x => x.IsAuthenticated).Returns(true);
        tenant.Setup(x => x.HasTenant).Returns(true);
        tenant.Setup(x => x.TenantId).Returns(tenantId);
        perm.Setup(x => x.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        custRepo.Setup(x => x.GetByIdAndTenantAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        prefRepo.Setup(x => x.GetByCustomerAndTenantAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pref);

        var mapper = AutoMapperTestHelper.CreateMapper();

        return new GetCustomerPreferenceUseCase(
            custRepo.Object,
            prefRepo.Object,
            user.Object,
            tenant.Object,
            perm.Object,
            mapper);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Empty_When_NotExists()
    {
        var customer = CustomerTestHelper.Create();

        var useCase = Build(customer, pref: null, out var tenantId);
        var result = await useCase.ExecuteAsync(customer.Id);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(Guid.Empty, result.Data!.Id);
        Assert.Equal(tenantId, result.Data.TenantId);
        Assert.Equal(customer.Id, result.Data.CustomerId);
        Assert.Null(result.Data.GeneralNotes);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Existing_Preference()
    {
        var customer = CustomerTestHelper.Create();
        var pref = CustomerPreferenceTestHelper.CreateWithData(tenantId: customer.TenantId, customerId: customer.Id);

        var useCase = Build(customer, pref, out _);
        var result = await useCase.ExecuteAsync(customer.Id);

        Assert.True(result.Success);
        Assert.Equal("LATAM, GOL", result.Data!.PreferredAirlines);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Customer_NotFound()
    {
        var useCase = Build(customer: null, pref: null, out _);
        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Equal("Cliente não encontrado.", result.Error);
    }
}
