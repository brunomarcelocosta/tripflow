using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tripflow.Application.DTOs.Requests.Travelers;
using Tripflow.Application.UseCases.Travelers;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.UseCases;

public class CreateTravelerUseCaseTests
{
    private static CreateTravelerRequest ValidRequest() => new(
        "Maria Souza",
        new DateOnly(1990, 1, 1),
        "Brasileira",
        "11122233344",
        "BR123456",
        new DateOnly(2035, 12, 31),
        "Notas");

    private static (Mock<ICustomerRepository> custRepo, Mock<ITravelerRepository> travRepo, Mock<IValidator<CreateTravelerRequest>> validator, Mock<IUserContext> user, Mock<ITenantContext> tenant, Mock<IUserPermissionService> perm, Guid tenantId) CreateMocks(Customer? customer)
    {
        var tenantId = customer?.TenantId ?? Guid.NewGuid();
        var custRepo = new Mock<ICustomerRepository>();
        var travRepo = new Mock<ITravelerRepository>();
        var validator = new Mock<IValidator<CreateTravelerRequest>>();
        var user = new Mock<IUserContext>();
        var tenant = new Mock<ITenantContext>();
        var perm = new Mock<IUserPermissionService>();

        user.Setup(x => x.IsAuthenticated).Returns(true);
        user.Setup(x => x.Email).Returns("user@test.com");
        tenant.Setup(x => x.HasTenant).Returns(true);
        tenant.Setup(x => x.TenantId).Returns(tenantId);
        perm.Setup(x => x.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        validator.Setup(x => x.ValidateAsync(It.IsAny<CreateTravelerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        custRepo.Setup(x => x.GetByIdAndTenantAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        travRepo.Setup(x => x.AddAsync(It.IsAny<Traveler>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        travRepo.Setup(x => x.ExistsPassportNumberAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        return (custRepo, travRepo, validator, user, tenant, perm, tenantId);
    }

    private static CreateTravelerUseCase Build(Customer? customer, Mock<ITravelerRepository>? travelerRepo = null)
    {
        var (custRepo, travRepo, validator, user, tenant, perm, _) = CreateMocks(customer);
        if (travelerRepo != null)
            travRepo = travelerRepo;

        var mapper = AutoMapperTestHelper.CreateMapper();
        return new CreateTravelerUseCase(custRepo.Object, travRepo.Object, validator.Object, user.Object, tenant.Object, perm.Object, mapper, NullLogger<CreateTravelerUseCase>.Instance);
    }

    [Fact]
    public async Task ExecuteAsync_Should_CreateTraveler_When_Customer_Found()
    {
        var customer = CustomerTestHelper.Create();
        var useCase = Build(customer);

        var result = await useCase.ExecuteAsync(customer.Id, ValidRequest());

        Assert.True(result.Success);
        Assert.Equal(customer.Id, result.Data!.CustomerId);
        Assert.Equal(customer.TenantId, result.Data.TenantId);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Fail_When_Customer_NotFound_Or_OtherTenant()
    {
        var useCase = Build(customer: null);

        var result = await useCase.ExecuteAsync(Guid.NewGuid(), ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("Cliente não encontrado.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Fail_When_Passport_Duplicate()
    {
        var customer = CustomerTestHelper.Create();
        var (custRepo, travRepo, validator, user, tenant, perm, _) = CreateMocks(customer);
        travRepo.Setup(x => x.ExistsPassportNumberAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var mapper = AutoMapperTestHelper.CreateMapper();
        var useCase = new CreateTravelerUseCase(custRepo.Object, travRepo.Object, validator.Object, user.Object, tenant.Object, perm.Object, mapper, NullLogger<CreateTravelerUseCase>.Instance);

        var result = await useCase.ExecuteAsync(customer.Id, ValidRequest());

        Assert.False(result.Success);
        Assert.Contains("passaporte", result.Error!);
    }
}
