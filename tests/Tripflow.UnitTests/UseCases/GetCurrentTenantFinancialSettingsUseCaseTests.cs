using Tripflow.UnitTests.Fixtures.UseCases;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.UseCases;

public class GetCurrentTenantFinancialSettingsUseCaseTests(GetCurrentTenantFinancialSettingsUseCaseFixture fixture)
    : IClassFixture<GetCurrentTenantFinancialSettingsUseCaseFixture>
{
    [Fact]
    public async Task ExecuteAsync_Should_Return_Mapped_Response_When_Configured()
    {
        var useCase = fixture.CreateForSuccess();

        var result = await useCase.ExecuteAsync();

        Assert.True(result.Success);
        Assert.Equal(fixture.CurrentTenantId, result.Data!.TenantId);
        Assert.Equal(100m, result.Data.DefaultProfitAmount);
        Assert.Null(result.Data.DefaultProfitPercentage);
        Assert.Equal(5m, result.Data.DefaultPixDiscountPercentage);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Empty_Response_When_Not_Configured()
    {
        var useCase = fixture.CreateForEmpty();

        var result = await useCase.ExecuteAsync();

        Assert.True(result.Success);
        Assert.Equal(fixture.CurrentTenantId, result.Data!.TenantId);
        Assert.Null(result.Data.DefaultProfitAmount);
        Assert.Null(result.Data.DefaultProfitPercentage);
        Assert.Null(result.Data.DefaultPixDiscountPercentage);
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
        Assert.Equal("Erro inesperado ao consultar configurações financeiras.", result.Error);
    }
}
