using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Tripflow.Application.DTOs.Requests.Users;
using Tripflow.Application.Services.Subscriptions;
using Tripflow.Application.UseCases.Users;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Integrations.Keycloak;
using Tripflow.Infra.Integrations.Keycloak.Interfaces;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Fixtures.UseCases;

public class InviteUserUseCaseFixture : IDisposable
{
    public Mock<IUserContext> MockUserContext { get; private set; } = null!;
    public Mock<ITenantContext> MockTenantContext { get; private set; } = null!;
    public Mock<IUserPermissionService> MockUserPermissionService { get; private set; } = null!;
    public Mock<IUserProfileRepository> MockUserProfileRepository { get; private set; } = null!;
    public Mock<IRoleRepository> MockRoleRepository { get; private set; } = null!;
    public Mock<IUserInvitationRepository> MockUserInvitationRepository { get; private set; } = null!;
    public Mock<IKeycloakUserService> MockKeycloakUserService { get; private set; } = null!;
    public Mock<IKeycloakRoleService> MockKeycloakRoleService { get; private set; } = null!;
    public Mock<IValidator<InviteUserRequest>> MockValidator { get; private set; } = null!;
    public Mock<ITenantUsageService> MockTenantUsageService { get; private set; } = null!;
    public IMapper Mapper { get; }
    public Guid TenantId { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    public InviteUserUseCaseFixture()
    {
        Mapper = AutoMapperTestHelper.CreateMapper();
        ResetMocks();
    }

    public InviteUserUseCase CreateUseCase() => new(
        MockUserContext.Object,
        MockTenantContext.Object,
        MockUserPermissionService.Object,
        MockUserProfileRepository.Object,
        MockRoleRepository.Object,
        MockUserInvitationRepository.Object,
        MockKeycloakUserService.Object,
        MockKeycloakRoleService.Object,
        Options.Create(new KeycloakOptions { Admin = new KeycloakAdminOptions { ManagedRolePrefix = "tripflow." } }),
        MockValidator.Object,
        MockTenantUsageService.Object,
        Mapper,
        NullLogger<InviteUserUseCase>.Instance);

    private void ResetMocks()
    {
        MockUserContext = new Mock<IUserContext>();
        MockTenantContext = new Mock<ITenantContext>();
        MockUserPermissionService = new Mock<IUserPermissionService>();
        MockUserProfileRepository = new Mock<IUserProfileRepository>();
        MockRoleRepository = new Mock<IRoleRepository>();
        MockUserInvitationRepository = new Mock<IUserInvitationRepository>();
        MockKeycloakUserService = new Mock<IKeycloakUserService>();
        MockKeycloakRoleService = new Mock<IKeycloakRoleService>();
        MockValidator = new Mock<IValidator<InviteUserRequest>>();
        MockTenantUsageService = new Mock<ITenantUsageService>();

        MockUserContext.Setup(x => x.IsAuthenticated).Returns(true);
        MockUserContext.Setup(x => x.Email).Returns("admin@test.com");
        MockTenantContext.Setup(x => x.HasTenant).Returns(true);
        MockTenantContext.Setup(x => x.TenantId).Returns(TenantId);
        MockUserPermissionService
            .Setup(x => x.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        MockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<InviteUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        MockTenantUsageService
            .Setup(x => x.HasAvailableLimitAsync(ITenantUsageService.Users, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        MockTenantUsageService
            .Setup(x => x.IncrementAsync(ITenantUsageService.Users, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        MockKeycloakUserService
            .Setup(x => x.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("keycloak-new-user");
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
            .Setup(x => x.AddAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        MockUserInvitationRepository
            .Setup(x => x.AddAsync(It.IsAny<UserInvitation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    public InviteUserUseCase CreateForSuccess(Role? role = null)
    {
        ResetMocks();
        role ??= IdentityTestHelper.CreateRole(TenantId, Tripflow.Infra.Data.Seeds.TripflowDbSeedData.Roles.Consultant);
        MockRoleRepository
            .Setup(x => x.GetByNamesAsync(TenantId, It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([role]);
        return CreateUseCase();
    }

    public InviteUserUseCase CreateForValidationFailure(string message = "Erro de validação.")
    {
        ResetMocks();
        MockValidator
            .Setup(x => x.ValidateAsync(It.IsAny<InviteUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("Email", message)]));
        return CreateUseCase();
    }

    public InviteUserUseCase CreateForForbidden()
    {
        ResetMocks();
        MockUserPermissionService
            .Setup(x => x.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return CreateUseCase();
    }

    public InviteUserUseCase CreateForKeycloakFailure()
    {
        ResetMocks();
        var role = IdentityTestHelper.CreateRole(TenantId);
        MockRoleRepository
            .Setup(x => x.GetByNamesAsync(TenantId, It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([role]);
        MockKeycloakUserService
            .Setup(x => x.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);
        return CreateUseCase();
    }

    public InviteUserUseCase CreateForRepositoryError()
    {
        ResetMocks();
        var role = IdentityTestHelper.CreateRole(TenantId);
        MockRoleRepository
            .Setup(x => x.GetByNamesAsync(TenantId, It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([role]);
        MockUserProfileRepository
            .Setup(x => x.AddAsync(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        return CreateUseCase();
    }

    public void Dispose() { }
}
