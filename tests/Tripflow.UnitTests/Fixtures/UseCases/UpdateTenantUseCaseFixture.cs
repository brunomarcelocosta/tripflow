using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.UseCases.Tenants;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Fixtures.UseCases;

public class UpdateTenantUseCaseFixture : IDisposable
{
    public Mock<ITenantRepository> MockRepository { get; private set; } = null!;
    public Mock<IValidator<UpdateTenantValidationRequest>> MockValidator { get; private set; } = null!;
    public Mock<IUserContext> MockUserContext { get; private set; } = null!;
    public Mock<IUserPermissionService> MockUserPermissionService { get; private set; } = null!;
    public IMapper Mapper { get; }
    public UpdateTenantUseCase UseCase { get; private set; } = null!;

    public UpdateTenantUseCaseFixture()
    {
        Mapper = AutoMapperTestHelper.CreateMapper();
        ResetMocks();
    }

    private void ResetMocks()
    {
        MockRepository = new Mock<ITenantRepository>();
        MockValidator = new Mock<IValidator<UpdateTenantValidationRequest>>();
        MockUserContext = new Mock<IUserContext>();
        MockUserContext.Setup(u => u.IsAuthenticated).Returns(true);
        MockUserContext.Setup(u => u.Email).Returns("test@test.com");
        MockUserContext.Setup(u => u.IdentityProviderUserId).Returns("user-123");
        MockUserPermissionService = new Mock<IUserPermissionService>();
        MockUserPermissionService
            .Setup(s => s.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        UseCase = new UpdateTenantUseCase(
            MockValidator.Object,
            MockRepository.Object,
            MockUserContext.Object,
            MockUserPermissionService.Object,
            Mapper,
            NullLogger<UpdateTenantUseCase>.Instance);
    }

    public UpdateTenantUseCase CreateForSuccess(Tenant? entity = null)
    {
        ResetMocks();
        var tenant = entity ?? TenantTestHelper.Create();
        SetupValidatorValid();
        MockRepository.Setup(r => r.GetByIdAsync(tenant.Id)).ReturnsAsync(tenant);
        SetupRepositorySuccess();
        return UseCase;
    }

    public UpdateTenantUseCase CreateForValidationFailure(string errorMessage = "Erro de validação.")
    {
        ResetMocks();
        SetupValidatorInvalid(errorMessage);
        return UseCase;
    }

    public UpdateTenantUseCase CreateForRepositoryError(Tenant? entity = null)
    {
        ResetMocks();
        var tenant = entity ?? TenantTestHelper.Create();
        SetupValidatorValid();
        MockRepository.Setup(r => r.GetByIdAsync(tenant.Id)).ReturnsAsync(tenant);
        MockRepository.Setup(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));
        MockRepository.Setup(r => r.RollbackTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        return UseCase;
    }

    public UpdateTenantUseCase CreateUseCaseWithIdentityProviderUserIdOnly(Tenant? entity = null)
    {
        ResetMocks();
        var tenant = entity ?? TenantTestHelper.Create();
        MockUserContext.Setup(u => u.Email).Returns((string?)null);
        MockUserContext.Setup(u => u.IdentityProviderUserId).Returns("keycloak-user-id");
        SetupValidatorValid();
        MockRepository.Setup(r => r.GetByIdAsync(tenant.Id)).ReturnsAsync(tenant);
        SetupRepositorySuccess();
        return UseCase;
    }

    private void SetupValidatorValid()
    {
        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateTenantValidationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupValidatorInvalid(string errorMessage)
    {
        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateTenantValidationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("LegalName", errorMessage)]));
    }

    private void SetupRepositorySuccess()
    {
        MockRepository.Setup(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
    }

    public void Dispose() { }
}
