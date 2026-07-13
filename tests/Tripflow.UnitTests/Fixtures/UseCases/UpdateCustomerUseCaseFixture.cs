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

namespace Tripflow.UnitTests.Fixtures.UseCases;

public class UpdateCustomerUseCaseFixture : IDisposable
{
    public Mock<ICustomerRepository> MockRepository { get; private set; } = null!;
    public Mock<ITravelerRepository> MockTravelerRepository { get; private set; } = null!;
    public Mock<IValidator<UpdateCustomerRequest>> MockValidator { get; private set; } = null!;
    public Mock<IUserContext> MockUserContext { get; private set; } = null!;
    public Mock<ITenantContext> MockTenantContext { get; private set; } = null!;
    public Mock<IUserPermissionService> MockUserPermissionService { get; private set; } = null!;
    public UpdateCustomerUseCase UseCase { get; private set; } = null!;

    public Guid CurrentTenantId { get; private set; } = Guid.NewGuid();

    public UpdateCustomerUseCaseFixture()
    {
        ResetMocks();
    }

    private void ResetMocks()
    {
        CurrentTenantId = Guid.NewGuid();
        MockRepository = new Mock<ICustomerRepository>();
        MockTravelerRepository = new Mock<ITravelerRepository>();
        MockValidator = new Mock<IValidator<UpdateCustomerRequest>>();
        MockUserContext = new Mock<IUserContext>();
        MockTenantContext = new Mock<ITenantContext>();
        MockUserPermissionService = new Mock<IUserPermissionService>();

        MockUserContext.Setup(x => x.IsAuthenticated).Returns(true);
        MockUserContext.Setup(x => x.Email).Returns("user@test.com");
        MockTenantContext.Setup(x => x.HasTenant).Returns(true);
        MockTenantContext.Setup(x => x.TenantId).Returns(CurrentTenantId);
        MockUserPermissionService
            .Setup(s => s.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateCustomerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        MockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockRepository
            .Setup(r => r.ExistsByDocumentNumberExceptIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        MockRepository
            .Setup(r => r.ExistsByEmailExceptIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        MockTravelerRepository
            .Setup(r => r.CountByCustomersAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, int>());

        UseCase = new UpdateCustomerUseCase(
            MockRepository.Object,
            MockTravelerRepository.Object,
            MockValidator.Object,
            MockUserContext.Object,
            MockTenantContext.Object,
            MockUserPermissionService.Object,
            NullLogger<UpdateCustomerUseCase>.Instance);
    }

    public (UpdateCustomerUseCase useCase, Customer customer) CreateForSuccess()
    {
        ResetMocks();
        var customer = CustomerTestHelper.Create(tenantId: CurrentTenantId);
        var customerId = customer.Id;
        MockRepository
            .Setup(r => r.GetTrackedByIdAndTenantAsync(customerId, CurrentTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        return (UseCase, customer);
    }

    public UpdateCustomerUseCase CreateForNotFound()
    {
        ResetMocks();
        MockRepository
            .Setup(r => r.GetTrackedByIdAndTenantAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);
        return UseCase;
    }

    public (UpdateCustomerUseCase useCase, Customer customer) CreateForCrossTenant()
    {
        ResetMocks();
        var customer = CustomerTestHelper.Create(tenantId: Guid.NewGuid());
        var customerId = customer.Id;
        MockRepository
            .Setup(r => r.GetTrackedByIdAndTenantAsync(customerId, CurrentTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);
        return (UseCase, customer);
    }

    public (UpdateCustomerUseCase useCase, Customer customer) CreateForDuplicateDocument()
    {
        ResetMocks();
        var customer = CustomerTestHelper.Create(tenantId: CurrentTenantId);
        var customerId = customer.Id;
        MockRepository
            .Setup(r => r.GetTrackedByIdAndTenantAsync(customerId, CurrentTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        MockRepository
            .Setup(r => r.ExistsByDocumentNumberExceptIdAsync(CurrentTenantId, It.IsAny<string>(), customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        return (UseCase, customer);
    }

    public void Dispose() { }
}
