using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.UseCases.Tenants;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Domain.Interfaces.Storage;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Fixtures.UseCases;

public class UploadTenantLogoUseCaseFixture : IDisposable
{
    public Mock<ITenantBrandingRepository> MockRepository { get; private set; } = null!;
    public Mock<IFileStorageService> MockFileStorageService { get; private set; } = null!;
    public Mock<IValidator<UploadTenantLogoRequest>> MockValidator { get; private set; } = null!;
    public Mock<IUserContext> MockUserContext { get; private set; } = null!;
    public Mock<ITenantContext> MockTenantContext { get; private set; } = null!;
    public Mock<IUserPermissionService> MockUserPermissionService { get; private set; } = null!;
    public UploadTenantLogoUseCase UseCase { get; private set; } = null!;

    public Guid CurrentTenantId { get; private set; } = Guid.NewGuid();
    public string UploadedUrl { get; private set; } = "/uploads/tenant-logos/test/logo.png";

    public UploadTenantLogoUseCaseFixture() => ResetMocks();

    private void ResetMocks()
    {
        CurrentTenantId = Guid.NewGuid();
        MockRepository = new Mock<ITenantBrandingRepository>();
        MockFileStorageService = new Mock<IFileStorageService>();
        MockValidator = new Mock<IValidator<UploadTenantLogoRequest>>();
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
        SetupStorageOk();

        UseCase = new UploadTenantLogoUseCase(
            MockRepository.Object,
            MockFileStorageService.Object,
            MockUserContext.Object,
            MockTenantContext.Object,
            MockUserPermissionService.Object,
            MockValidator.Object,
            NullLogger<UploadTenantLogoUseCase>.Instance);
    }

    public UploadTenantLogoUseCase CreateForCreate()
    {
        ResetMocks();
        MockRepository
            .Setup(r => r.GetTrackedByTenantIdAsync(CurrentTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantBranding?)null);
        return UseCase;
    }

    public UploadTenantLogoUseCase CreateForUpdate(TenantBranding? existing = null)
    {
        ResetMocks();
        var branding = existing ?? TenantBrandingTestHelper.Create(tenantId: CurrentTenantId);
        MockRepository
            .Setup(r => r.GetTrackedByTenantIdAsync(CurrentTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branding);
        return UseCase;
    }

    public UploadTenantLogoUseCase CreateForValidationFailure(string errorMessage = "Erro de validação.")
    {
        ResetMocks();
        SetupValidatorInvalid(errorMessage);
        return UseCase;
    }

    public UploadTenantLogoUseCase CreateForUnauthenticated()
    {
        ResetMocks();
        MockUserContext.Setup(x => x.IsAuthenticated).Returns(false);
        return UseCase;
    }

    public UploadTenantLogoUseCase CreateForNoTenant()
    {
        ResetMocks();
        MockTenantContext.Setup(x => x.HasTenant).Returns(false);
        MockTenantContext.Setup(x => x.TenantId).Returns((Guid?)null);
        return UseCase;
    }

    public UploadTenantLogoUseCase CreateForNoPermission()
    {
        ResetMocks();
        MockUserPermissionService
            .Setup(s => s.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return UseCase;
    }

    public UploadTenantLogoUseCase CreateForStorageError()
    {
        ResetMocks();
        MockFileStorageService
            .Setup(s => s.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new IOException("Disk full"));
        return UseCase;
    }

    public UploadTenantLogoUseCase CreateForRepositoryError()
    {
        ResetMocks();
        MockRepository
            .Setup(r => r.GetTrackedByTenantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        return UseCase;
    }

    public static UploadTenantLogoRequest BuildRequest(
        string fileName = "logo.png",
        string contentType = "image/png",
        long sizeBytes = 1024)
    {
        return new UploadTenantLogoRequest
        {
            Content = new MemoryStream([0x1, 0x2, 0x3]),
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = sizeBytes
        };
    }

    private void SetupValidatorValid()
    {
        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UploadTenantLogoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupValidatorInvalid(string errorMessage)
    {
        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UploadTenantLogoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("FileName", errorMessage)]));
    }

    private void SetupTransactionsOk()
    {
        MockRepository.Setup(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.CommitTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.RollbackTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.AddAsync(It.IsAny<TenantBranding>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.UpdateAsync(It.IsAny<TenantBranding>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
    }

    private void SetupStorageOk()
    {
        MockFileStorageService
            .Setup(s => s.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(UploadedUrl);
    }

    public void Dispose() { }
}
