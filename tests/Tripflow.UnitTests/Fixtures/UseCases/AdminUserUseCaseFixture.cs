using FluentValidation;
using FluentValidation.Results;
using Moq;
using Tripflow.Application.DTOs.Requests.Admin;
using Tripflow.Application.Services.Audit;
using Tripflow.Application.UseCases.Admin;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Integrations.Keycloak.Interfaces;

namespace Tripflow.UnitTests.Fixtures.UseCases;

public sealed class AdminUserUseCaseFixture
{
    public Mock<IUserContext> MockUserContext { get; } = new();
    public Mock<IUserProfileRepository> MockUserProfileRepository { get; } = new();
    public Mock<IKeycloakUserService> MockKeycloakUserService { get; } = new();
    public Mock<IAuditService> MockAuditService { get; } = new();
    public Mock<IValidator<UpdateAdminUserRequest>> MockUpdateValidator { get; } = new();
    public Mock<IValidator<SetAdminUserPasswordRequest>> MockPasswordValidator { get; } = new();

    public Guid UserId { get; } = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public Guid TenantId { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    public AdminUserUseCaseFixture()
    {
        MockUserContext.Setup(x => x.IsAuthenticated).Returns(true);
        MockUserContext.Setup(x => x.Email).Returns("admin@test.com");
        MockUpdateValidator
            .Setup(x => x.ValidateAsync(It.IsAny<UpdateAdminUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        MockPasswordValidator
            .Setup(x => x.ValidateAsync(It.IsAny<SetAdminUserPasswordRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    public UserProfile CreateUser(UserStatus status = UserStatus.Active)
    {
        var tenant = new Tenant("Legal", "Trade", null, "tenant@test.com", null, TenantStatus.Active, "seed");
        typeof(Tenant).GetProperty(nameof(Tenant.Id))!.SetValue(tenant, TenantId);

        var user = new UserProfile(TenantId, "keycloak-123", "Usuário Teste", "user@test.com", "11999999999", status, "seed");
        typeof(UserProfile).GetProperty(nameof(UserProfile.Id))!.SetValue(user, UserId);
        typeof(UserProfile).GetProperty(nameof(UserProfile.Tenant))!.SetValue(user, tenant);
        return user;
    }

    public GetAdminUsersUseCase CreateGetAdminUsersUseCase()
        => new(MockUserProfileRepository.Object, MockUserContext.Object);

    public UpdateAdminUserUseCase CreateUpdateAdminUserUseCase()
        => new(
            MockUserProfileRepository.Object,
            MockKeycloakUserService.Object,
            MockAuditService.Object,
            MockUpdateValidator.Object,
            MockUserContext.Object);

    public SetAdminUserPasswordUseCase CreateSetAdminUserPasswordUseCase()
        => new(
            MockUserProfileRepository.Object,
            MockKeycloakUserService.Object,
            MockAuditService.Object,
            MockPasswordValidator.Object,
            MockUserContext.Object);
}
