using AutoMapper;
using Moq;
using Tripflow.Application.UseCases.Roles;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Fixtures.UseCases;

public class GetRolesUseCaseFixture : IDisposable
{
    public Mock<ITenantContext> MockTenantContext { get; private set; } = null!;
    public Mock<IUserContext> MockUserContext { get; private set; } = null!;
    public Mock<IUserPermissionService> MockUserPermissionService { get; private set; } = null!;
    public Mock<IRoleRepository> MockRoleRepository { get; private set; } = null!;
    public IMapper Mapper { get; }
    public Guid TenantId { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    public GetRolesUseCaseFixture()
    {
        Mapper = AutoMapperTestHelper.CreateMapper();
        ResetMocks();
    }

    public GetRolesUseCase CreateUseCase() => new(
        MockTenantContext.Object,
        MockUserContext.Object,
        MockUserPermissionService.Object,
        MockRoleRepository.Object,
        Mapper);

    private void ResetMocks()
    {
        MockTenantContext = new Mock<ITenantContext>();
        MockUserContext = new Mock<IUserContext>();
        MockUserPermissionService = new Mock<IUserPermissionService>();
        MockRoleRepository = new Mock<IRoleRepository>();

        MockUserContext.Setup(x => x.IsAuthenticated).Returns(true);
        MockTenantContext.Setup(x => x.HasTenant).Returns(true);
        MockTenantContext.Setup(x => x.TenantId).Returns(TenantId);
        MockUserPermissionService
            .Setup(x => x.HasPermissionAsync(Tripflow.Infra.Data.Seeds.TripflowDbSeedData.Permissions.UsersRead, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    public GetRolesUseCase CreateForSuccess(List<Role>? roles = null)
    {
        ResetMocks();
        roles ??=
        [
            IdentityTestHelper.CreateRole(TenantId, "Consultant", permissions: IdentityTestHelper.CreatePermission("users.read")),
            IdentityTestHelper.CreateRole(TenantId, "AgencyAdmin", permissions: IdentityTestHelper.CreatePermission("users.manage"))
        ];
        MockRoleRepository
            .Setup(x => x.GetAllByTenantAsync(TenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);
        return CreateUseCase();
    }

    public GetRolesUseCase CreateForForbidden()
    {
        ResetMocks();
        MockUserPermissionService
            .Setup(x => x.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return CreateUseCase();
    }

    public void Dispose() { }
}
