using Tripflow.UnitTests.Fixtures.UseCases;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.UseCases;

public class GetCurrentTenantBrandingUseCaseTests(GetCurrentTenantBrandingUseCaseFixture fixture)
    : IClassFixture<GetCurrentTenantBrandingUseCaseFixture>
{
    [Fact]
    public async Task ExecuteAsync_Should_Return_Ok_With_Mapped_Response()
    {
        var branding = TenantBrandingTestHelper.Create(tenantId: fixture.CurrentTenantId);
        var useCase = fixture.CreateForSuccess(branding);

        var result = await useCase.ExecuteAsync();

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(branding.Id, result.Data!.Id);
        Assert.Equal(branding.PrimaryColor, result.Data.PrimaryColor);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Empty_Response_When_No_Branding()
    {
        var useCase = fixture.CreateForEmptyBranding();

        var result = await useCase.ExecuteAsync();

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(Guid.Empty, result.Data!.Id);
        Assert.Equal(fixture.CurrentTenantId, result.Data.TenantId);
        Assert.Null(result.Data.LogoUrl);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Forbidden_When_Not_Authenticated()
    {
        var useCase = fixture.CreateForUnauthenticated();

        var result = await useCase.ExecuteAsync();

        Assert.False(result.Success);
        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_No_Tenant_Resolved()
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

        Assert.False(result.Success);
        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Repository_Throws()
    {
        var useCase = fixture.CreateForRepositoryError();

        var result = await useCase.ExecuteAsync();

        Assert.False(result.Success);
        Assert.Equal("Erro inesperado ao consultar branding.", result.Error);
    }
}
