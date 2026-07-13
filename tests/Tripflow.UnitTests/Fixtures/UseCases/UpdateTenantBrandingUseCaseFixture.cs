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

public class UpdateTenantBrandingUseCaseFixture : IDisposable
{
    public Mock<ITenantBrandingRepository> MockRepository { get; private set; } = null!;
    public Mock<IValidator<UpdateTenantBrandingRequest>> MockValidator { get; private set; } = null!;
    public Mock<IUserContext> MockUserContext { get; private set; } = null!;
    public Mock<ITenantContext> MockTenantContext { get; private set; } = null!;
    public Mock<IUserPermissionService> MockUserPermissionService { get; private set; } = null!;
    public IMapper Mapper { get; }
    public UpdateTenantBrandingUseCase UseCase { get; private set; } = null!;

    public Guid CurrentTenantId { get; private set; } = Guid.NewGuid();

    public UpdateTenantBrandingUseCaseFixture()
    {
        Mapper = AutoMapperTestHelper.CreateMapper();
        ResetMocks();
    }

    private void ResetMocks()
    {
        CurrentTenantId = Guid.NewGuid();
        MockRepository = new Mock<ITenantBrandingRepository>();
        MockValidator = new Mock<IValidator<UpdateTenantBrandingRequest>>();
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

        SetupValidatorValid();
        SetupTransactionsOk();

        UseCase = new UpdateTenantBrandingUseCase(
            MockRepository.Object,
            MockUserContext.Object,
            MockTenantContext.Object,
            MockUserPermissionService.Object,
            MockValidator.Object,
            Mapper,
            NullLogger<UpdateTenantBrandingUseCase>.Instance);
    }

    public UpdateTenantBrandingUseCase CreateForCreate()
    {
        ResetMocks();
        MockRepository
            .Setup(r => r.GetTrackedByTenantIdAsync(CurrentTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantBranding?)null);
        return UseCase;
    }

    public UpdateTenantBrandingUseCase CreateForUpdate(TenantBranding? existing = null)
    {
        ResetMocks();
        var entity = existing ?? TenantBrandingTestHelper.Create(tenantId: CurrentTenantId);
        MockRepository
            .Setup(r => r.GetTrackedByTenantIdAsync(CurrentTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        return UseCase;
    }

    public UpdateTenantBrandingUseCase CreateForValidationFailure(string errorMessage = "Erro de validação.")
    {
        ResetMocks();
        SetupValidatorInvalid(errorMessage);
        return UseCase;
    }

    public UpdateTenantBrandingUseCase CreateForUnauthenticated()
    {
        ResetMocks();
        MockUserContext.Setup(x => x.IsAuthenticated).Returns(false);
        return UseCase;
    }

    public UpdateTenantBrandingUseCase CreateForNoTenant()
    {
        ResetMocks();
        MockTenantContext.Setup(x => x.HasTenant).Returns(false);
        MockTenantContext.Setup(x => x.TenantId).Returns((Guid?)null);
        return UseCase;
    }

    public UpdateTenantBrandingUseCase CreateForNoPermission()
    {
        ResetMocks();
        MockUserPermissionService
            .Setup(s => s.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return UseCase;
    }

    public UpdateTenantBrandingUseCase CreateForRepositoryError()
    {
        ResetMocks();
        MockRepository
            .Setup(r => r.GetTrackedByTenantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        return UseCase;
    }

    private void SetupValidatorValid()
    {
        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateTenantBrandingRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupValidatorInvalid(string errorMessage)
    {
        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateTenantBrandingRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("PrimaryColor", errorMessage)]));
    }

    private void SetupTransactionsOk()
    {
        MockRepository.Setup(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.CommitTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.RollbackTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.AddAsync(It.IsAny<TenantBranding>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.UpdateAsync(It.IsAny<TenantBranding>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
    }

    public void Dispose() { }
}
