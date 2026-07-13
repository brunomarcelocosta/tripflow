using Moq;
using Tripflow.Application.DTOs.Requests.Pricing;
using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Enums;
using Tripflow.UnitTests.Fixtures.UseCases;

namespace Tripflow.UnitTests.UseCases;

public class UpdatePaymentFeeRulesUseCaseTests(UpdatePaymentFeeRulesUseCaseFixture fixture)
    : IClassFixture<UpdatePaymentFeeRulesUseCaseFixture>
{
    private static UpdatePaymentFeeRulesRequest RequestWith(params UpdatePaymentFeeRuleItem[] items) =>
        new() { Rules = items };

    [Fact]
    public async Task ExecuteAsync_Should_Create_New_Rules_When_None_Exist()
    {
        var useCase = fixture.CreateForCreateNew();
        var request = RequestWith(
            new UpdatePaymentFeeRuleItem { PaymentMethod = PaymentMethod.Pix, Installments = 1, FeePercentage = 0m, IsActive = true },
            new UpdatePaymentFeeRuleItem { PaymentMethod = PaymentMethod.CreditCard, Installments = 3, FeePercentage = 5.5m, IsActive = true });

        var result = await useCase.ExecuteAsync(request);

        Assert.True(result.Success);
        fixture.MockRepository.Verify(r => r.AddAsync(It.IsAny<PaymentFeeRule>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        fixture.MockRepository.Verify(r => r.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Update_Existing_Rule_On_Upsert()
    {
        var existing = new PaymentFeeRule(fixture.CurrentTenantId, PaymentMethod.CreditCard, 1, 3m, true, "system");
        var useCase = fixture.CreateForUpsert(existing);
        var request = RequestWith(
            new UpdatePaymentFeeRuleItem { PaymentMethod = PaymentMethod.CreditCard, Installments = 1, FeePercentage = 4.5m, IsActive = false });

        var result = await useCase.ExecuteAsync(request);

        Assert.True(result.Success);
        Assert.Equal(4.5m, existing.FeePercentage);
        Assert.False(existing.IsActive);
        fixture.MockRepository.Verify(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
        fixture.MockRepository.Verify(r => r.AddAsync(It.IsAny<PaymentFeeRule>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Forbidden_When_Not_Authenticated()
    {
        var useCase = fixture.CreateForUnauthenticated();
        var request = RequestWith(new UpdatePaymentFeeRuleItem { PaymentMethod = PaymentMethod.Pix, Installments = 1, FeePercentage = 0m });

        var result = await useCase.ExecuteAsync(request);

        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_No_Tenant()
    {
        var useCase = fixture.CreateForNoTenant();
        var request = RequestWith(new UpdatePaymentFeeRuleItem { PaymentMethod = PaymentMethod.Pix, Installments = 1, FeePercentage = 0m });

        var result = await useCase.ExecuteAsync(request);

        Assert.False(result.Success);
        Assert.Equal("Tenant não resolvido para o usuário atual.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Forbidden_When_Missing_Permission()
    {
        var useCase = fixture.CreateForNoPermission();
        var request = RequestWith(new UpdatePaymentFeeRuleItem { PaymentMethod = PaymentMethod.Pix, Installments = 1, FeePercentage = 0m });

        var result = await useCase.ExecuteAsync(request);

        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Validation_Fails()
    {
        var useCase = fixture.CreateForValidationFailure("A lista de regras é obrigatória.");
        var request = RequestWith();

        var result = await useCase.ExecuteAsync(request);

        Assert.False(result.Success);
        Assert.Equal("A lista de regras é obrigatória.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Rollback_When_Repository_Throws()
    {
        var useCase = fixture.CreateForRepositoryError();
        var request = RequestWith(new UpdatePaymentFeeRuleItem { PaymentMethod = PaymentMethod.Pix, Installments = 1, FeePercentage = 0m });

        var result = await useCase.ExecuteAsync(request);

        Assert.False(result.Success);
        Assert.Equal("Erro inesperado ao atualizar regras de taxa.", result.Error);
        fixture.MockRepository.Verify(r => r.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
