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

public class UpdateTenantCommercialSettingsUseCaseFixture : IDisposable
{
    public Mock<ITenantCommercialSettingsRepository> MockRepository { get; private set; } = null!;
    public Mock<IValidator<UpdateTenantCommercialSettingsRequest>> MockValidator { get; private set; } = null!;
    public Mock<IUserContext> MockUserContext { get; private set; } = null!;
    public Mock<ITenantContext> MockTenantContext { get; private set; } = null!;
    public Mock<IUserPermissionService> MockUserPermissionService { get; private set; } = null!;
    public IMapper Mapper { get; }
    public UpdateTenantCommercialSettingsUseCase UseCase { get; private set; } = null!;

    public Guid CurrentTenantId { get; private set; } = Guid.NewGuid();

    public UpdateTenantCommercialSettingsUseCaseFixture()
    {
        Mapper = AutoMapperTestHelper.CreateMapper();
        ResetMocks();
    }

    private void ResetMocks()
    {
        CurrentTenantId = Guid.NewGuid();
        MockRepository = new Mock<ITenantCommercialSettingsRepository>();
        MockValidator = new Mock<IValidator<UpdateTenantCommercialSettingsRequest>>();
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

        UseCase = new UpdateTenantCommercialSettingsUseCase(
            MockRepository.Object,
            MockUserContext.Object,
            MockTenantContext.Object,
            MockUserPermissionService.Object,
            MockValidator.Object,
            Mapper,
            NullLogger<UpdateTenantCommercialSettingsUseCase>.Instance);
    }

    public UpdateTenantCommercialSettingsUseCase CreateForCreate()
    {
        ResetMocks();
        MockRepository
            .Setup(r => r.GetTrackedByTenantIdAsync(CurrentTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantCommercialSettings?)null);
        return UseCase;
    }

    public UpdateTenantCommercialSettingsUseCase CreateForUpdate(TenantCommercialSettings? existing = null)
    {
        ResetMocks();
        var entity = existing ?? TenantCommercialSettingsTestHelper.CreateWithCommercialData(tenantId: CurrentTenantId);
        MockRepository
            .Setup(r => r.GetTrackedByTenantIdAsync(CurrentTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        return UseCase;
    }

    public UpdateTenantCommercialSettingsUseCase CreateForValidationFailure(string errorMessage = "Erro de validação.")
    {
        ResetMocks();
        SetupValidatorInvalid(errorMessage);
        return UseCase;
    }

    public UpdateTenantCommercialSettingsUseCase CreateForUnauthenticated()
    {
        ResetMocks();
        MockUserContext.Setup(x => x.IsAuthenticated).Returns(false);
        return UseCase;
    }

    public UpdateTenantCommercialSettingsUseCase CreateForNoTenant()
    {
        ResetMocks();
        MockTenantContext.Setup(x => x.HasTenant).Returns(false);
        MockTenantContext.Setup(x => x.TenantId).Returns((Guid?)null);
        return UseCase;
    }

    public UpdateTenantCommercialSettingsUseCase CreateForNoPermission()
    {
        ResetMocks();
        MockUserPermissionService
            .Setup(s => s.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return UseCase;
    }

    public UpdateTenantCommercialSettingsUseCase CreateForRepositoryError()
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
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateTenantCommercialSettingsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupValidatorInvalid(string errorMessage)
    {
        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateTenantCommercialSettingsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("CommercialEmail", errorMessage)]));
    }

    private void SetupTransactionsOk()
    {
        MockRepository.Setup(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.CommitTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.RollbackTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.AddAsync(It.IsAny<TenantCommercialSettings>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.UpdateAsync(It.IsAny<TenantCommercialSettings>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
    }

    public void Dispose() { }
}
