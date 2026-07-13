using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Tripflow.Application.DTOs.Requests.Roles;
using Tripflow.Application.UseCases.Roles;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Integrations.Keycloak;
using Tripflow.Infra.Integrations.Keycloak.Interfaces;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Fixtures.UseCases;

public class AssignUserRolesUseCaseFixture : IDisposable
{
    public Mock<ITenantContext> MockTenantContext { get; private set; } = null!;
    public Mock<IUserContext> MockUserContext { get; private set; } = null!;
    public Mock<IUserPermissionService> MockUserPermissionService { get; private set; } = null!;
    public Mock<IUserProfileRepository> MockUserProfileRepository { get; private set; } = null!;
    public Mock<IRoleRepository> MockRoleRepository { get; private set; } = null!;
    public Mock<IKeycloakRoleService> MockKeycloakRoleService { get; private set; } = null!;
    public Mock<IValidator<AssignUserRolesValidationRequest>> MockValidator { get; private set; } = null!;
    public IMapper Mapper { get; }
    public Guid TenantId { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public Guid UserId { get; } = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    public AssignUserRolesUseCaseFixture()
    {
        Mapper = AutoMapperTestHelper.CreateMapper();
        ResetMocks();
    }

    public AssignUserRolesUseCase CreateUseCase() => new(
        MockTenantContext.Object,
        MockUserContext.Object,
        MockUserPermissionService.Object,
        MockUserProfileRepository.Object,
        MockRoleRepository.Object,
        MockKeycloakRoleService.Object,
        Options.Create(new KeycloakOptions { Admin = new KeycloakAdminOptions { ManagedRolePrefix = "tripflow." } }),
        MockValidator.Object,
        Mapper,
        NullLogger<AssignUserRolesUseCase>.Instance);

    private void ResetMocks()
    {
        MockTenantContext = new Mock<ITenantContext>();
        MockUserContext = new Mock<IUserContext>();
        MockUserPermissionService = new Mock<IUserPermissionService>();
        MockUserProfileRepository = new Mock<IUserProfileRepository>();
        MockRoleRepository = new Mock<IRoleRepository>();
        MockKeycloakRoleService = new Mock<IKeycloakRoleService>();
        MockValidator = new Mock<IValidator<AssignUserRolesValidationRequest>>();

        MockUserContext.Setup(x => x.IsAuthenticated).Returns(true);
        MockTenantContext.Setup(x => x.HasTenant).Returns(true);
        MockTenantContext.Setup(x => x.TenantId).Returns(TenantId);
        MockUserPermissionService
            .Setup(x => x.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        MockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<AssignUserRolesValidationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        MockKeycloakRoleService
            .Setup(x => x.SyncRealmRolesAsync(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        MockUserProfileRepository
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockUserProfileRepository
            .Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockUserProfileRepository
            .Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockUserProfileRepository
            .Setup(x => x.UpdateAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    public AssignUserRolesUseCase CreateForSuccess(UserProfile? profile = null, Role? role = null)
    {
        ResetMocks();
        role ??= IdentityTestHelper.CreateRole(TenantId, Tripflow.Infra.Data.Seeds.TripflowDbSeedData.Roles.Consultant);
        profile ??= IdentityTestHelper.CreateUserProfile(tenantId: TenantId);
        MockRoleRepository
            .Setup(x => x.GetByNamesAsync(TenantId, It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([role]);
        MockUserProfileRepository
            .Setup(x => x.GetTrackedByIdInTenantAsync(TenantId, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        MockRoleRepository
            .Setup(x => x.GetByIdsAsync(TenantId, It.IsAny<IReadOnlyList<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([role]);
        return CreateUseCase();
    }

    public AssignUserRolesUseCase CreateForValidationFailure(string message = "Erro de validação.")
    {
        ResetMocks();
        MockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<AssignUserRolesValidationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("UserId", message)]));
        return CreateUseCase();
    }

    public AssignUserRolesUseCase CreateForRepositoryError(UserProfile? profile = null, Role? role = null)
    {
        ResetMocks();
        role ??= IdentityTestHelper.CreateRole(TenantId);
        profile ??= IdentityTestHelper.CreateUserProfile(tenantId: TenantId);
        MockRoleRepository
            .Setup(x => x.GetByNamesAsync(TenantId, It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([role]);
        MockUserProfileRepository
            .Setup(x => x.GetTrackedByIdInTenantAsync(TenantId, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        MockUserProfileRepository
            .Setup(x => x.UpdateAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        return CreateUseCase();
    }

    public void Dispose() { }
}
