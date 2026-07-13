using Tripflow.UnitTests.Fixtures.UseCases;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.UseCases;

public class GetCurrentTenantCommercialSettingsUseCaseTests(GetCurrentTenantCommercialSettingsUseCaseFixture fixture)
    : IClassFixture<GetCurrentTenantCommercialSettingsUseCaseFixture>
{
    [Fact]
    public async Task ExecuteAsync_Should_Return_Mapped_Response_When_Settings_Exist()
    {
        var entity = TenantCommercialSettingsTestHelper.CreateWithCommercialData(
            tenantId: fixture.CurrentTenantId,
            commercialEmail: "comercial@test.com");
        var useCase = fixture.CreateForSuccess(entity);

        var result = await useCase.ExecuteAsync();

        Assert.True(result.Success);
        Assert.Equal(entity.Id, result.Data!.Id);
        Assert.Equal("comercial@test.com", result.Data.CommercialEmail);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Default_Response_When_Not_Configured()
    {
        var useCase = fixture.CreateForEmpty();

        var result = await useCase.ExecuteAsync();

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(Guid.Empty, result.Data!.Id);
        Assert.Equal(fixture.CurrentTenantId, result.Data.TenantId);
        Assert.Equal(24, result.Data.DefaultProposalExpirationHours);
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
        Assert.Equal("Erro inesperado ao consultar configurações comerciais.", result.Error);
    }
}
