using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tripflow.Application.UseCases.Tenants;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Fixtures.UseCases;

public class GetCurrentTenantBrandingUseCaseFixture : IDisposable
{
    public Mock<ITenantBrandingRepository> MockRepository { get; private set; } = null!;
    public Mock<IUserContext> MockUserContext { get; private set; } = null!;
    public Mock<ITenantContext> MockTenantContext { get; private set; } = null!;
    public Mock<IUserPermissionService> MockUserPermissionService { get; private set; } = null!;
    public IMapper Mapper { get; }
    public GetCurrentTenantBrandingUseCase UseCase { get; private set; } = null!;

    public Guid CurrentTenantId { get; private set; } = Guid.NewGuid();

    public GetCurrentTenantBrandingUseCaseFixture()
    {
        Mapper = AutoMapperTestHelper.CreateMapper();
        ResetMocks();
    }

    private void ResetMocks()
    {
        CurrentTenantId = Guid.NewGuid();
        MockRepository = new Mock<ITenantBrandingRepository>();
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

        UseCase = new GetCurrentTenantBrandingUseCase(
            MockRepository.Object,
            MockUserContext.Object,
            MockTenantContext.Object,
            MockUserPermissionService.Object,
            Mapper,
            NullLogger<GetCurrentTenantBrandingUseCase>.Instance);
    }

    public GetCurrentTenantBrandingUseCase CreateForSuccess(TenantBranding? entity = null)
    {
        ResetMocks();
        var branding = entity ?? TenantBrandingTestHelper.Create(tenantId: CurrentTenantId);
        MockRepository
            .Setup(r => r.GetByTenantIdAsync(CurrentTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branding);
        return UseCase;
    }

    public GetCurrentTenantBrandingUseCase CreateForEmptyBranding()
    {
        ResetMocks();
        MockRepository
            .Setup(r => r.GetByTenantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantBranding?)null);
        return UseCase;
    }

    public GetCurrentTenantBrandingUseCase CreateForUnauthenticated()
    {
        ResetMocks();
        MockUserContext.Setup(x => x.IsAuthenticated).Returns(false);
        return UseCase;
    }

    public GetCurrentTenantBrandingUseCase CreateForNoTenant()
    {
        ResetMocks();
        MockTenantContext.Setup(x => x.HasTenant).Returns(false);
        MockTenantContext.Setup(x => x.TenantId).Returns((Guid?)null);
        return UseCase;
    }

    public GetCurrentTenantBrandingUseCase CreateForNoPermission()
    {
        ResetMocks();
        MockUserPermissionService
            .Setup(s => s.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return UseCase;
    }

    public GetCurrentTenantBrandingUseCase CreateForRepositoryError()
    {
        ResetMocks();
        MockRepository
            .Setup(r => r.GetByTenantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        return UseCase;
    }

    public void Dispose() { }
}
