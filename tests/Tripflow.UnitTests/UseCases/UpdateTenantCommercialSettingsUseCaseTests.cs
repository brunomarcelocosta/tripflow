using Moq;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.UnitTests.Fixtures.UseCases;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.UseCases;

public class UpdateTenantCommercialSettingsUseCaseTests(UpdateTenantCommercialSettingsUseCaseFixture fixture)
    : IClassFixture<UpdateTenantCommercialSettingsUseCaseFixture>
{
    private static UpdateTenantCommercialSettingsRequest ValidRequest() => new()
    {
        CommercialEmail = "novo@empresa.com",
        CommercialPhone = "11990001111",
        WhatsApp = "11988887777",
        Instagram = "@empresa",
        Website = "https://empresa.com",
        DefaultTerms = "Termos atualizados.",
        DefaultImportantNotes = "Notas atualizadas.",
        DefaultProposalExpirationHours = 72
    };

    [Fact]
    public async Task ExecuteAsync_Should_Create_When_Not_Exists()
    {
        var useCase = fixture.CreateForCreate();

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.True(result.Success);
        Assert.Equal("novo@empresa.com", result.Data!.CommercialEmail);
        fixture.MockRepository.Verify(
            r => r.AddAsync(It.Is<TenantCommercialSettings>(s => s.CommercialEmail == "novo@empresa.com"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Update_When_Exists()
    {
        var existing = TenantCommercialSettingsTestHelper.CreateWithCommercialData(tenantId: fixture.CurrentTenantId);
        var useCase = fixture.CreateForUpdate(existing);

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.True(result.Success);
        Assert.Equal("novo@empresa.com", existing.CommercialEmail);
        Assert.Equal(72, existing.DefaultProposalExpirationHours);
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
        var useCase = fixture.CreateForValidationFailure("O e-mail comercial informado é inválido.");

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("O e-mail comercial informado é inválido.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Rollback_When_Repository_Throws()
    {
        var useCase = fixture.CreateForRepositoryError();

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("Erro inesperado ao atualizar configurações comerciais.", result.Error);
        fixture.MockRepository.Verify(r => r.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
