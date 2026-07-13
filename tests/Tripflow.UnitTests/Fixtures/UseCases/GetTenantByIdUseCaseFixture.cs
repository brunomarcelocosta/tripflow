using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.UseCases.Tenants;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Fixtures.UseCases;

public class GetTenantByIdUseCaseFixture : IDisposable
{
    public Mock<ITenantRepository> MockRepository { get; private set; } = null!;
    public Mock<IUserContext> MockUserContext { get; private set; } = null!;
    public Mock<IUserPermissionService> MockUserPermissionService { get; private set; } = null!;
    public Mock<IValidator<GetTenantByIdRequest>> MockValidator { get; private set; } = null!;
    public IMapper Mapper { get; }
    public GetTenantByIdUseCase UseCase { get; private set; } = null!;

    public GetTenantByIdUseCaseFixture()
    {
        Mapper = AutoMapperTestHelper.CreateMapper();
        ResetMocks();
    }

    private void ResetMocks()
    {
        MockRepository = new Mock<ITenantRepository>();
        MockUserContext = new Mock<IUserContext>();
        MockUserPermissionService = new Mock<IUserPermissionService>();
        MockValidator = new Mock<IValidator<GetTenantByIdRequest>>();
        MockUserContext.Setup(u => u.IsAuthenticated).Returns(true);
        MockUserPermissionService
            .Setup(s => s.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        UseCase = new GetTenantByIdUseCase(
            MockRepository.Object,
            MockUserContext.Object,
            MockUserPermissionService.Object,
            MockValidator.Object,
            Mapper);
    }

    public GetTenantByIdUseCase CreateForSuccess(Tenant? entity = null)
    {
        ResetMocks();
        var tenant = entity ?? TenantTestHelper.Create();
        SetupValidatorValid();
        MockRepository.Setup(r => r.GetByIdAsync(tenant.Id)).ReturnsAsync(tenant);
        return UseCase;
    }

    public GetTenantByIdUseCase CreateForValidationFailure(string errorMessage = "Erro de validação.")
    {
        ResetMocks();
        SetupValidatorInvalid(errorMessage);
        return UseCase;
    }

    public GetTenantByIdUseCase CreateForNotFound()
    {
        ResetMocks();
        SetupValidatorValid();
        MockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Tenant?)null);
        return UseCase;
    }

    public GetTenantByIdUseCase CreateForValidationFailureWithoutMessage()
    {
        ResetMocks();
        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<GetTenantByIdRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("Id", "") { ErrorMessage = null }]));
        return UseCase;
    }

    private void SetupValidatorValid()
    {
        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<GetTenantByIdRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupValidatorInvalid(string errorMessage)
    {
        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<GetTenantByIdRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("Id", errorMessage)]));
    }

    public void Dispose() { }
}
