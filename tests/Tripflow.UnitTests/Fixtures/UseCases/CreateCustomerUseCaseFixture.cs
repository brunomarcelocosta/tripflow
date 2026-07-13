using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Application.UseCases.Customers;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.UnitTests.Fixtures.UseCases;

public class CreateCustomerUseCaseFixture : IDisposable
{
    public Mock<ICustomerRepository> MockRepository { get; private set; } = null!;
    public Mock<IValidator<CreateCustomerRequest>> MockValidator { get; private set; } = null!;
    public Mock<IUserContext> MockUserContext { get; private set; } = null!;
    public Mock<ITenantContext> MockTenantContext { get; private set; } = null!;
    public Mock<IUserPermissionService> MockUserPermissionService { get; private set; } = null!;
    public CreateCustomerUseCase UseCase { get; private set; } = null!;

    public Guid CurrentTenantId { get; private set; } = Guid.NewGuid();

    public CreateCustomerUseCaseFixture()
    {
        ResetMocks();
    }

    private void ResetMocks()
    {
        CurrentTenantId = Guid.NewGuid();
        MockRepository = new Mock<ICustomerRepository>();
        MockValidator = new Mock<IValidator<CreateCustomerRequest>>();
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
            .Setup(v => v.ValidateAsync(It.IsAny<CreateCustomerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        MockRepository
            .Setup(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockRepository
            .Setup(r => r.ExistsByDocumentNumberAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        MockRepository
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        UseCase = new CreateCustomerUseCase(
            MockRepository.Object,
            MockValidator.Object,
            MockUserContext.Object,
            MockTenantContext.Object,
            MockUserPermissionService.Object,
            NullLogger<CreateCustomerUseCase>.Instance);
    }

    public CreateCustomerUseCase CreateForSuccess()
    {
        ResetMocks();
        return UseCase;
    }

    public CreateCustomerUseCase CreateForUnauthenticated()
    {
        ResetMocks();
        MockUserContext.Setup(x => x.IsAuthenticated).Returns(false);
        return UseCase;
    }

    public CreateCustomerUseCase CreateForNoTenant()
    {
        ResetMocks();
        MockTenantContext.Setup(x => x.HasTenant).Returns(false);
        MockTenantContext.Setup(x => x.TenantId).Returns((Guid?)null);
        return UseCase;
    }

    public CreateCustomerUseCase CreateForNoPermission()
    {
        ResetMocks();
        MockUserPermissionService
            .Setup(s => s.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return UseCase;
    }

    public CreateCustomerUseCase CreateForValidationFailure(string error = "Erro de validação.")
    {
        ResetMocks();
        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateCustomerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("FullName", error) }));
        return UseCase;
    }

    public CreateCustomerUseCase CreateForDuplicateDocument()
    {
        ResetMocks();
        MockRepository
            .Setup(r => r.ExistsByDocumentNumberAsync(CurrentTenantId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        return UseCase;
    }

    public CreateCustomerUseCase CreateForDuplicateEmail()
    {
        ResetMocks();
        MockRepository
            .Setup(r => r.ExistsByEmailAsync(CurrentTenantId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        return UseCase;
    }

    public CreateCustomerUseCase CreateForRepositoryError()
    {
        ResetMocks();
        MockRepository
            .Setup(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        return UseCase;
    }

    public void Dispose() { }
}
