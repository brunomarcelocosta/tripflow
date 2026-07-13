using Tripflow.Domain.Entities.Pricing;
using Tripflow.Domain.Enums;
using Tripflow.UnitTests.Fixtures.UseCases;

namespace Tripflow.UnitTests.UseCases;

public class GetPaymentFeeRulesUseCaseTests(GetPaymentFeeRulesUseCaseFixture fixture)
    : IClassFixture<GetPaymentFeeRulesUseCaseFixture>
{
    [Fact]
    public async Task ExecuteAsync_Should_Return_Mapped_Rules_When_Found()
    {
        var rules = new List<PaymentFeeRule>
        {
            new(fixture.CurrentTenantId, PaymentMethod.Pix, 1, 0m, true, "system"),
            new(fixture.CurrentTenantId, PaymentMethod.CreditCard, 3, 5.9m, true, "system"),
        };
        var useCase = fixture.CreateForSuccess(rules);

        var result = await useCase.ExecuteAsync();

        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.Count);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Empty_When_None_Configured()
    {
        var useCase = fixture.CreateForEmpty();

        var result = await useCase.ExecuteAsync();

        Assert.True(result.Success);
        Assert.Empty(result.Data!);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Forbidden_When_Not_Authenticated()
    {
        var useCase = fixture.CreateForUnauthenticated();

        var result = await useCase.ExecuteAsync();

        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_No_Tenant()
    {
        var useCase = fixture.CreateForNoTenant();

        var result = await useCase.ExecuteAsync();

        Assert.False(result.Success);
        Assert.Equal("Tenant não resolvido para o usuário atual.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Forbidden_When_Missing_Permission()
    {
        var useCase = fixture.CreateForNoPermission();

        var result = await useCase.ExecuteAsync();

        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Repository_Throws()
    {
        var useCase = fixture.CreateForRepositoryError();

        var result = await useCase.ExecuteAsync();

        Assert.False(result.Success);
        Assert.Equal("Erro inesperado ao consultar regras de taxa.", result.Error);
    }
}
