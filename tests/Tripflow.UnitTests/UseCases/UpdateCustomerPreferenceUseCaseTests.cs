using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Application.UseCases.Customers;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.UseCases;

public class UpdateCustomerPreferenceUseCaseTests
{
    private static UpdateCustomerPreferenceRequest ValidRequest() => new(
        "LATAM",
        "Resort",
        "Janela",
        "Vegetariano",
        "Cruzeiros",
        "Cliente VIP");

    private static UpdateCustomerPreferenceUseCase Build(Customer? customer, CustomerPreference? existingPreference, out Mock<ICustomerPreferenceRepository> prefRepo, out Guid tenantId)
    {
        tenantId = customer?.TenantId ?? Guid.NewGuid();
        var custRepo = new Mock<ICustomerRepository>();
        prefRepo = new Mock<ICustomerPreferenceRepository>();
        var validator = new Mock<IValidator<UpdateCustomerPreferenceRequest>>();
        var user = new Mock<IUserContext>();
        var tenant = new Mock<ITenantContext>();
        var perm = new Mock<IUserPermissionService>();

        user.Setup(x => x.IsAuthenticated).Returns(true);
        user.Setup(x => x.Email).Returns("user@test.com");
        tenant.Setup(x => x.HasTenant).Returns(true);
        tenant.Setup(x => x.TenantId).Returns(tenantId);
        perm.Setup(x => x.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        validator.Setup(x => x.ValidateAsync(It.IsAny<UpdateCustomerPreferenceRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        custRepo.Setup(x => x.GetByIdAndTenantAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        prefRepo.Setup(x => x.GetTrackedByCustomerAndTenantAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPreference);
        prefRepo.Setup(x => x.AddAsync(It.IsAny<CustomerPreference>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        prefRepo.Setup(x => x.UpdateAsync(It.IsAny<CustomerPreference>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var mapper = AutoMapperTestHelper.CreateMapper();

        return new UpdateCustomerPreferenceUseCase(
            custRepo.Object,
            prefRepo.Object,
            validator.Object,
            user.Object,
            tenant.Object,
            perm.Object,
            mapper,
            NullLogger<UpdateCustomerPreferenceUseCase>.Instance);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Create_Preference_When_Not_Exists()
    {
        var customer = CustomerTestHelper.Create();
        var useCase = Build(customer, existingPreference: null, out var prefRepo, out _);

        var result = await useCase.ExecuteAsync(customer.Id, ValidRequest());

        Assert.True(result.Success);
        prefRepo.Verify(r => r.AddAsync(It.IsAny<CustomerPreference>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Update_When_Exists()
    {
        var customer = CustomerTestHelper.Create();
        var pref = CustomerPreferenceTestHelper.Create(tenantId: customer.TenantId, customerId: customer.Id);

        var useCase = Build(customer, existingPreference: pref, out var prefRepo, out _);

        var result = await useCase.ExecuteAsync(customer.Id, ValidRequest());

        Assert.True(result.Success);
        prefRepo.Verify(r => r.UpdateAsync(It.IsAny<CustomerPreference>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Fail_When_Customer_NotFound()
    {
        var useCase = Build(customer: null, existingPreference: null, out _, out _);

        var result = await useCase.ExecuteAsync(Guid.NewGuid(), ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("Cliente não encontrado.", result.Error);
    }
}
