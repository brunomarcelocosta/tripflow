using Moq;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.UnitTests.Fixtures.UseCases;
using Tripflow.UnitTests.Utils;

namespace Tripflow.UnitTests.UseCases;

public class UpdateTenantUseCaseTests(UpdateTenantUseCaseFixture fixture) : IClassFixture<UpdateTenantUseCaseFixture>
{
    private static UpdateTenantRequest ValidRequest() => new(
        "Razão Social Atualizada LTDA",
        "Nome Fantasia Atualizado",
        "12345678000199",
        "updated@test.com",
        "11988887777",
        TenantStatus.Inactive);

    [Fact]
    public async Task ExecuteAsync_Should_Return_Ok_With_Response_When_Valid_Request()
    {
        var tenant = TenantTestHelper.Create();
        var useCase = fixture.CreateForSuccess(tenant);
        var request = ValidRequest();

        var result = await useCase.ExecuteAsync(tenant.Id, request);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(tenant.Id, result.Data!.Id);
        Assert.Equal(request.LegalName, result.Data.LegalName);
        Assert.Equal(request.TradeName, result.Data.TradeName);
        Assert.Equal(request.Status, result.Data.Status);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Validation_Fails()
    {
        var tenant = TenantTestHelper.Create();
        var useCase = fixture.CreateForValidationFailure("A razão social é obrigatória.");
        var request = ValidRequest();

        var result = await useCase.ExecuteAsync(tenant.Id, request);

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("A razão social é obrigatória.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Repository_Throws()
    {
        var tenant = TenantTestHelper.Create();
        var useCase = fixture.CreateForRepositoryError(tenant);
        var request = ValidRequest();

        var result = await useCase.ExecuteAsync(tenant.Id, request);

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Erro inesperado ao atualizar tenant.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_UpdateAsync_With_Updated_Entity()
    {
        var tenant = TenantTestHelper.Create();
        var useCase = fixture.CreateForSuccess(tenant);
        var request = ValidRequest();

        await useCase.ExecuteAsync(tenant.Id, request);

        fixture.MockRepository.Verify(r => r.UpdateAsync(It.Is<Tenant>(t =>
            t.LegalName == request.LegalName &&
            t.TradeName == request.TradeName &&
            t.DocumentNumber == request.DocumentNumber &&
            t.Email == request.Email &&
            t.Phone == request.Phone &&
            t.Status == request.Status), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_RollbackTransaction_When_Repository_Throws()
    {
        var tenant = TenantTestHelper.Create();
        var useCase = fixture.CreateForRepositoryError(tenant);
        var request = ValidRequest();

        await useCase.ExecuteAsync(tenant.Id, request);

        fixture.MockRepository.Verify(r => r.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Not_Call_UpdateAsync_When_Validation_Fails()
    {
        var tenant = TenantTestHelper.Create();
        var useCase = fixture.CreateForValidationFailure("Erro");
        var request = ValidRequest();

        await useCase.ExecuteAsync(tenant.Id, request);

        fixture.MockRepository.Verify(r => r.UpdateAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_BeginTransaction_When_Success()
    {
        var tenant = TenantTestHelper.Create();
        var useCase = fixture.CreateForSuccess(tenant);
        var request = ValidRequest();

        await useCase.ExecuteAsync(tenant.Id, request);

        fixture.MockRepository.Verify(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Use_IdentityProviderUserId_When_Email_Is_Null()
    {
        var tenant = TenantTestHelper.Create();
        var useCase = fixture.CreateUseCaseWithIdentityProviderUserIdOnly(tenant);
        var request = ValidRequest();

        await useCase.ExecuteAsync(tenant.Id, request);

        fixture.MockRepository.Verify(
            r => r.UpdateAsync(It.Is<Tenant>(t => t.UpdatedBy == "keycloak-user-id"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Validate_Using_UpdateTenantValidationRequest_From()
    {
        var tenant = TenantTestHelper.Create();
        var useCase = fixture.CreateForSuccess(tenant);
        var request = ValidRequest();

        await useCase.ExecuteAsync(tenant.Id, request);

        fixture.MockValidator.Verify(
            v => v.ValidateAsync(
                It.Is<UpdateTenantValidationRequest>(r =>
                    r.Id == tenant.Id &&
                    r.LegalName == request.LegalName &&
                    r.TradeName == request.TradeName &&
                    r.Status == request.Status),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
