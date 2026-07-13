using Moq;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.UnitTests.Fixtures.UseCases;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.UseCases;

public class UploadTenantLogoUseCaseTests(UploadTenantLogoUseCaseFixture fixture)
    : IClassFixture<UploadTenantLogoUseCaseFixture>
{
    [Fact]
    public async Task ExecuteAsync_Should_Create_Branding_When_Not_Exists()
    {
        var useCase = fixture.CreateForCreate();

        var result = await useCase.ExecuteAsync(UploadTenantLogoUseCaseFixture.BuildRequest());

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(fixture.UploadedUrl, result.Data!.LogoUrl);
        Assert.Equal(fixture.CurrentTenantId, result.Data.TenantId);
        fixture.MockRepository.Verify(
            r => r.AddAsync(It.Is<TenantBranding>(b => b.LogoUrl == fixture.UploadedUrl), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Update_Existing_Branding_Logo()
    {
        var existing = TenantBrandingTestHelper.Create(tenantId: fixture.CurrentTenantId, logoUrl: "/old.png");
        var useCase = fixture.CreateForUpdate(existing);

        var result = await useCase.ExecuteAsync(UploadTenantLogoUseCaseFixture.BuildRequest());

        Assert.True(result.Success);
        Assert.Equal(fixture.UploadedUrl, existing.LogoUrl);
        fixture.MockRepository.Verify(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Forbidden_When_Not_Authenticated()
    {
        var useCase = fixture.CreateForUnauthenticated();

        var result = await useCase.ExecuteAsync(UploadTenantLogoUseCaseFixture.BuildRequest());

        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_No_Tenant()
    {
        var useCase = fixture.CreateForNoTenant();

        var result = await useCase.ExecuteAsync(UploadTenantLogoUseCaseFixture.BuildRequest());

        Assert.False(result.Success);
        Assert.Equal("Tenant não resolvido para o usuário atual.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Forbidden_When_Missing_Permission()
    {
        var useCase = fixture.CreateForNoPermission();

        var result = await useCase.ExecuteAsync(UploadTenantLogoUseCaseFixture.BuildRequest());

        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Validation_Fails()
    {
        var useCase = fixture.CreateForValidationFailure("Extensão do arquivo não permitida. Use .png, .jpg, .jpeg, .webp ou .svg.");

        var result = await useCase.ExecuteAsync(UploadTenantLogoUseCaseFixture.BuildRequest(fileName: "logo.exe"));

        Assert.False(result.Success);
        Assert.Equal("Extensão do arquivo não permitida. Use .png, .jpg, .jpeg, .webp ou .svg.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Storage_Throws()
    {
        var useCase = fixture.CreateForStorageError();

        var result = await useCase.ExecuteAsync(UploadTenantLogoUseCaseFixture.BuildRequest());

        Assert.False(result.Success);
        Assert.Equal("Erro inesperado ao salvar o logo da empresa.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Rollback_When_Repository_Throws()
    {
        var useCase = fixture.CreateForRepositoryError();

        var result = await useCase.ExecuteAsync(UploadTenantLogoUseCaseFixture.BuildRequest());

        Assert.False(result.Success);
        Assert.Equal("Erro inesperado ao atualizar o logo da empresa.", result.Error);
        fixture.MockRepository.Verify(r => r.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
