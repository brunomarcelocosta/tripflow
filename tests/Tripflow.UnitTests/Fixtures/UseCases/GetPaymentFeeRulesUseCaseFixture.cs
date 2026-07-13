using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tripflow.Application.UseCases.Pricing;
using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.Fixtures.UseCases;

public class GetPaymentFeeRulesUseCaseFixture : IDisposable
{
    public Mock<IPaymentFeeRuleRepository> MockRepository { get; private set; } = null!;
    public Mock<IUserContext> MockUserContext { get; private set; } = null!;
    public Mock<ITenantContext> MockTenantContext { get; private set; } = null!;
    public Mock<IUserPermissionService> MockUserPermissionService { get; private set; } = null!;
    public IMapper Mapper { get; }
    public GetPaymentFeeRulesUseCase UseCase { get; private set; } = null!;

    public Guid CurrentTenantId { get; private set; } = Guid.NewGuid();

    public GetPaymentFeeRulesUseCaseFixture()
    {
        Mapper = AutoMapperTestHelper.CreateMapper();
        ResetMocks();
    }

    private void ResetMocks()
    {
        CurrentTenantId = Guid.NewGuid();
        MockRepository = new Mock<IPaymentFeeRuleRepository>();
        MockUserContext = new Mock<IUserContext>();
        MockTenantContext = new Mock<ITenantContext>();
        MockUserPermissionService = new Mock<IUserPermissionService>();

        MockUserContext.Setup(x => x.IsAuthenticated).Returns(true);
        MockTenantContext.Setup(x => x.HasTenant).Returns(true);
        MockTenantContext.Setup(x => x.TenantId).Returns(CurrentTenantId);
        MockUserPermissionService
            .Setup(s => s.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        UseCase = new GetPaymentFeeRulesUseCase(
            MockRepository.Object,
            MockUserContext.Object,
            MockTenantContext.Object,
            MockUserPermissionService.Object,
            Mapper,
            NullLogger<GetPaymentFeeRulesUseCase>.Instance);
    }

    public GetPaymentFeeRulesUseCase CreateForSuccess(List<PaymentFeeRule>? rules = null)
    {
        ResetMocks();
        var data = rules ?? new List<PaymentFeeRule>
        {
            new(CurrentTenantId, PaymentMethod.Pix, 1, 0m, true, "system"),
            new(CurrentTenantId, PaymentMethod.CreditCard, 1, 4.2m, true, "system"),
        };
        MockRepository
            .Setup(r => r.GetByTenantIdAsync(CurrentTenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);
        return UseCase;
    }

    public GetPaymentFeeRulesUseCase CreateForEmpty()
    {
        ResetMocks();
        MockRepository
            .Setup(r => r.GetByTenantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        return UseCase;
    }

    public GetPaymentFeeRulesUseCase CreateForUnauthenticated()
    {
        ResetMocks();
        MockUserContext.Setup(x => x.IsAuthenticated).Returns(false);
        return UseCase;
    }

    public GetPaymentFeeRulesUseCase CreateForNoTenant()
    {
        ResetMocks();
        MockTenantContext.Setup(x => x.HasTenant).Returns(false);
        MockTenantContext.Setup(x => x.TenantId).Returns((Guid?)null);
        return UseCase;
    }

    public GetPaymentFeeRulesUseCase CreateForNoPermission()
    {
        ResetMocks();
        MockUserPermissionService
            .Setup(s => s.HasPermissionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return UseCase;
    }

    public GetPaymentFeeRulesUseCase CreateForRepositoryError()
    {
        ResetMocks();
        MockRepository
            .Setup(r => r.GetByTenantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        return UseCase;
    }

    public void Dispose() { }
}
