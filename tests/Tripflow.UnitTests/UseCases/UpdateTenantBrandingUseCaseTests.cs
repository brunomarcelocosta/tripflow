using Moq;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.UnitTests.Fixtures.UseCases;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.UseCases;

public class UpdateTenantBrandingUseCaseTests(UpdateTenantBrandingUseCaseFixture fixture)
    : IClassFixture<UpdateTenantBrandingUseCaseFixture>
{
    private static UpdateTenantBrandingRequest ValidRequest() => new()
    {
        PrimaryColor = "#111111",
        SecondaryColor = "#222222",
        TextColor = "#333333",
        ProposalFooter = "Rodapé atualizado"
    };

    [Fact]
    public async Task ExecuteAsync_Should_Create_Branding_When_Not_Exists()
    {
        var useCase = fixture.CreateForCreate();

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("#111111", result.Data!.PrimaryColor);
        fixture.MockRepository.Verify(
            r => r.AddAsync(It.Is<TenantBranding>(b => b.TenantId == fixture.CurrentTenantId), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Update_Branding_When_Exists()
    {
        var existing = TenantBrandingTestHelper.Create(tenantId: fixture.CurrentTenantId);
        var useCase = fixture.CreateForUpdate(existing);

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.True(result.Success);
        Assert.Equal("#111111", existing.PrimaryColor);
        Assert.Equal("Rodapé atualizado", existing.ProposalFooter);
        fixture.MockRepository.Verify(
            r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Forbidden_When_Not_Authenticated()
    {
        var useCase = fixture.CreateForUnauthenticated();

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Tenant_Not_Resolved()
    {
        var useCase = fixture.CreateForNoTenant();

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("Tenant não resolvido para o usuário atual.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Forbidden_When_Missing_Permission()
    {
        var useCase = fixture.CreateForNoPermission();

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Validation_Fails()
    {
        var useCase = fixture.CreateForValidationFailure("A cor primária deve estar no formato HEX (ex.: #FFFFFF).");

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("A cor primária deve estar no formato HEX (ex.: #FFFFFF).", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Rollback_When_Repository_Throws()
    {
        var useCase = fixture.CreateForRepositoryError();

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("Erro inesperado ao atualizar branding.", result.Error);
        fixture.MockRepository.Verify(r => r.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
