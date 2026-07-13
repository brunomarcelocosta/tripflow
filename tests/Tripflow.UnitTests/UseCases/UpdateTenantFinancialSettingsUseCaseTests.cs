using Moq;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.UnitTests.Fixtures.UseCases;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.UseCases;

public class UpdateTenantFinancialSettingsUseCaseTests(UpdateTenantFinancialSettingsUseCaseFixture fixture)
    : IClassFixture<UpdateTenantFinancialSettingsUseCaseFixture>
{
    private static UpdateTenantFinancialSettingsRequest ValidRequest() => new()
    {
        DefaultProfitAmount = 150m,
        DefaultProfitPercentage = null,
        DefaultPixDiscountPercentage = 7m
    };

    [Fact]
    public async Task ExecuteAsync_Should_Create_When_Not_Exists()
    {
        var useCase = fixture.CreateForCreate();

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.True(result.Success);
        Assert.Equal(150m, result.Data!.DefaultProfitAmount);
        fixture.MockRepository.Verify(
            r => r.AddAsync(It.Is<TenantCommercialSettings>(s => s.DefaultProfitAmount == 150m), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Update_When_Exists()
    {
        var existing = TenantCommercialSettingsTestHelper.CreateWithFinancialData(tenantId: fixture.CurrentTenantId);
        var useCase = fixture.CreateForUpdate(existing);

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.True(result.Success);
        Assert.Equal(150m, existing.DefaultProfitAmount);
        Assert.Equal(7m, existing.DefaultPixDiscountPercentage);
        fixture.MockRepository.Verify(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Forbidden_When_Not_Authenticated()
    {
        var useCase = fixture.CreateForUnauthenticated();

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_No_Tenant()
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
        var useCase = fixture.CreateForValidationFailure("A margem padrão em valor não pode ser negativa.");

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("A margem padrão em valor não pode ser negativa.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Rollback_When_Repository_Throws()
    {
        var useCase = fixture.CreateForRepositoryError();

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("Erro inesperado ao atualizar configurações financeiras.", result.Error);
        fixture.MockRepository.Verify(r => r.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
