using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tripflow.Application.DTOs.Requests.Pricing;
using Tripflow.Application.UseCases.Pricing;
using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Fixtures.UseCases;

public class UpdatePaymentFeeRulesUseCaseFixture : IDisposable
{
    public Mock<IPaymentFeeRuleRepository> MockRepository { get; private set; } = null!;
    public Mock<IValidator<UpdatePaymentFeeRulesRequest>> MockValidator { get; private set; } = null!;
    public Mock<IUserContext> MockUserContext { get; private set; } = null!;
    public Mock<ITenantContext> MockTenantContext { get; private set; } = null!;
    public Mock<IUserPermissionService> MockUserPermissionService { get; private set; } = null!;
    public IMapper Mapper { get; }
    public UpdatePaymentFeeRulesUseCase UseCase { get; private set; } = null!;

    public Guid CurrentTenantId { get; private set; } = Guid.NewGuid();

    public UpdatePaymentFeeRulesUseCaseFixture()
    {
        Mapper = AutoMapperTestHelper.CreateMapper();
        ResetMocks();
    }

    private void ResetMocks()
    {
        CurrentTenantId = Guid.NewGuid();
        MockRepository = new Mock<IPaymentFeeRuleRepository>();
        MockValidator = new Mock<IValidator<UpdatePaymentFeeRulesRequest>>();
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

        UseCase = new UpdatePaymentFeeRulesUseCase(
            MockRepository.Object,
            MockUserContext.Object,
            MockTenantContext.Object,
            MockUserPermissionService.Object,
            MockValidator.Object,
            Mapper,
            NullLogger<UpdatePaymentFeeRulesUseCase>.Instance);
    }

    public UpdatePaymentFeeRulesUseCase CreateForCreateNew(List<PaymentFeeRule>? finalState = null)
    {
        ResetMocks();
        MockRepository
            .Setup(r => r.GetTrackedByTenantPaymentMethodAndInstallmentsAsync(
                CurrentTenantId, It.IsAny<PaymentMethod>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaymentFeeRule?)null);

        MockRepository
            .Setup(r => r.GetByTenantIdAsync(CurrentTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(finalState ?? []);
        return UseCase;
    }

    public UpdatePaymentFeeRulesUseCase CreateForUpsert(PaymentFeeRule existing, List<PaymentFeeRule>? finalState = null)
    {
        ResetMocks();
        MockRepository
            .Setup(r => r.GetTrackedByTenantPaymentMethodAndInstallmentsAsync(
                CurrentTenantId, existing.PaymentMethod, existing.Installments, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        MockRepository
            .Setup(r => r.GetByTenantIdAsync(CurrentTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(finalState ?? [existing]);
        return UseCase;
    }

    public UpdatePaymentFeeRulesUseCase CreateForValidationFailure(string errorMessage = "Erro de validação.")
    {
        ResetMocks();
        SetupValidatorInvalid(errorMessage);
        return UseCase;
    }

    public UpdatePaymentFeeRulesUseCase CreateForUnauthenticated()
    {
        ResetMocks();
        MockUserContext.Setup(x => x.IsAuthenticated).Returns(false);
        return UseCase;
    }

    public UpdatePaymentFeeRulesUseCase CreateForNoTenant()
    {
        ResetMocks();
        MockTenantContext.Setup(x => x.HasTenant).Returns(false);
        MockTenantContext.Setup(x => x.TenantId).Returns((Guid?)null);
        return UseCase;
    }

    public UpdatePaymentFeeRulesUseCase CreateForNoPermission()
    {
        ResetMocks();
        MockUserPermissionService
            .Setup(s => s.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return UseCase;
    }

    public UpdatePaymentFeeRulesUseCase CreateForRepositoryError()
    {
        ResetMocks();
        MockRepository
            .Setup(r => r.GetTrackedByTenantPaymentMethodAndInstallmentsAsync(
                It.IsAny<Guid>(), It.IsAny<PaymentMethod>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        return UseCase;
    }

    private void SetupValidatorValid()
    {
        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdatePaymentFeeRulesRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupValidatorInvalid(string errorMessage)
    {
        MockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdatePaymentFeeRulesRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("Rules", errorMessage)]));
    }

    private void SetupTransactionsOk()
    {
        MockRepository.Setup(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.CommitTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.RollbackTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.AddAsync(It.IsAny<PaymentFeeRule>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MockRepository.Setup(r => r.UpdateAsync(It.IsAny<PaymentFeeRule>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
    }

    public void Dispose() { }
}
