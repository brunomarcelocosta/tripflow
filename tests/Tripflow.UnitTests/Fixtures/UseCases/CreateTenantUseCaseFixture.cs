using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.UseCases.Tenants;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;

namespace Tripflow.UnitTests.Fixtures.UseCases;

public class CreateTenantUseCaseFixture : IDisposable
{
    public Mock<ITenantRepository> MockRepository { get; private set; } = null!;
    public Mock<ITenantRoleProvisioningService> MockTenantRoleProvisioning { get; private set; } = null!;
    public Mock<IUserPermissionService> MockUserPermissionService { get; private set; } = null!;
    public Mock<IValidator<CreateTenantRequest>> MockValidator { get; private set; } = null!;
    public Mock<IUserContext> MockUserContext { get; private set; } = null!;
    public CreateTenantUseCase UseCase { get; private set; } = null!;

    private void ResetMocks()
    {
        MockRepository = new Mock<ITenantRepository>();
        MockTenantRoleProvisioning = new Mock<ITenantRoleProvisioningService>();
        MockUserPermissionService = new Mock<IUserPermissionService>();
        MockValidator = new Mock<IValidator<CreateTenantRequest>>();
        MockUserContext = new Mock<IUserContext>();
        MockUserContext.Setup(u => u.IsAuthenticated).Returns(true);
        MockUserContext.Setup(u => u.Email).Returns("test@test.com");
        MockUserContext.Setup(u => u.IdentityProviderUserId).Returns("user-123");
        MockUserPermissionService
            .Setup(s => s.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        MockTenantRoleProvisioning
            .Setup(s => s.ProvisionDefaultRolesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        UseCase = new CreateTenantUseCase(
            MockRepository.Object,
            MockTenantRoleProvisioning.Object,
            MockUserPermissionService.Object,
            MockValidator.Object,
            MockUserContext.Object,
            NullLogger<CreateTenantUseCase>.Instance);
    }

    public CreateTenantUseCase CreateUseCaseForSuccess()
    {
        ResetMocks();
        SetupValidatorValid();
        SetupRepositorySuccess();
        return UseCase;
    }

    public CreateTenantUseCase CreateUseCaseForValidationFailure(string errorMessage = "Erro de validação.")
    {
        ResetMocks();
        SetupValidatorInvalid(errorMessage);
        return UseCase;
    }

    public CreateTenantUseCase CreateUseCaseForValidationFailureWithDefaultMessage()
    {
        ResetMocks();
        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateTenantRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("LegalName", "") { ErrorMessage = null }]));
        return UseCase;
    }

    public CreateTenantUseCase CreateUseCaseForRepositoryError()
    {
        ResetMocks();
        SetupValidatorValid();
        SetupRepositoryThrows();
        return UseCase;
    }

    public CreateTenantUseCase CreateUseCaseWithIdentityProviderUserIdOnly()
    {
        ResetMocks();
        MockUserContext.Setup(u => u.Email).Returns((string?)null);
        MockUserContext.Setup(u => u.IdentityProviderUserId).Returns("keycloak-user-id");
        SetupValidatorValid();
        SetupRepositorySuccess();
        return UseCase;
    }

    public CreateTenantUseCase CreateUseCaseWithSystemUser()
    {
        ResetMocks();
        MockUserContext.Setup(u => u.Email).Returns((string?)null);
        MockUserContext.Setup(u => u.IdentityProviderUserId).Returns((string?)null);
        SetupValidatorValid();
        SetupRepositorySuccess();
        return UseCase;
    }

    private void SetupValidatorValid()
    {
        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateTenantRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupValidatorInvalid(string errorMessage)
    {
        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateTenantRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("LegalName", errorMessage)]));
    }

    private void SetupRepositorySuccess()
    {
        MockRepository.Setup(repo => repo.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(repo => repo.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(repo => repo.CommitTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
    }

    private void SetupRepositoryThrows()
    {
        MockRepository.Setup(repo => repo.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(repo => repo.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));
        MockRepository.Setup(repo => repo.RollbackTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
    }

    public void Dispose() { }
}
