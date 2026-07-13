using Moq;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.UnitTests.Fixtures.UseCases;

namespace Tripflow.UnitTests.UseCases;

public class CreateTenantUseCaseTests(CreateTenantUseCaseFixture fixture) : IClassFixture<CreateTenantUseCaseFixture>
{
    private static CreateTenantRequest ValidRequest() => new(
        "Razão Social LTDA",
        "Nome Fantasia",
        "12345678000199",
        "tenant@test.com",
        "11999999999");

    [Fact]
    public async Task ExecuteAsync_Should_Return_Ok_With_Id_When_Valid_Request()
    {
        var useCase = fixture.CreateUseCaseForSuccess();
        var request = ValidRequest();

        var result = await useCase.ExecuteAsync(request);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotEqual(Guid.Empty, result.Data!.Value);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Validation_Fails()
    {
        var useCase = fixture.CreateUseCaseForValidationFailure("A razão social é obrigatória.");
        var request = ValidRequest();

        var result = await useCase.ExecuteAsync(request);

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("A razão social é obrigatória.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Default_Validation_Message_When_Error_Message_Is_Null()
    {
        var useCase = fixture.CreateUseCaseForValidationFailureWithDefaultMessage();
        var request = ValidRequest();

        var result = await useCase.ExecuteAsync(request);

        Assert.False(result.Success);
        Assert.Equal("Erro de validação.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Repository_Throws()
    {
        var useCase = fixture.CreateUseCaseForRepositoryError();
        var request = ValidRequest();

        var result = await useCase.ExecuteAsync(request);

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Erro inesperado ao criar tenant.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_BeginTransaction_Once()
    {
        var useCase = fixture.CreateUseCaseForSuccess();
        var request = ValidRequest();

        await useCase.ExecuteAsync(request);

        fixture.MockRepository.Verify(
            repo => repo.BeginTransactionAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_AddAsync_With_Correct_Tenant()
    {
        var useCase = fixture.CreateUseCaseForSuccess();
        var request = ValidRequest();

        await useCase.ExecuteAsync(request);

        fixture.MockRepository.Verify(
            repo => repo.AddAsync(It.Is<Tenant>(t =>
                t.LegalName == request.LegalName &&
                t.TradeName == request.TradeName &&
                t.DocumentNumber == request.DocumentNumber &&
                t.Email == request.Email &&
                t.Phone == request.Phone &&
                t.Status == TenantStatus.Active), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_CommitTransaction_When_Success()
    {
        var useCase = fixture.CreateUseCaseForSuccess();
        var request = ValidRequest();

        await useCase.ExecuteAsync(request);

        fixture.MockRepository.Verify(
            repo => repo.CommitTransactionAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_RollbackTransaction_When_Repository_Throws()
    {
        var useCase = fixture.CreateUseCaseForRepositoryError();
        var request = ValidRequest();

        await useCase.ExecuteAsync(request);

        fixture.MockRepository.Verify(
            repo => repo.RollbackTransactionAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Not_Call_AddAsync_When_Validation_Fails()
    {
        var useCase = fixture.CreateUseCaseForValidationFailure("Erro");
        var request = ValidRequest();

        await useCase.ExecuteAsync(request);

        fixture.MockRepository.Verify(
            repo => repo.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Use_IdentityProviderUserId_When_Email_Is_Null()
    {
        var useCase = fixture.CreateUseCaseWithIdentityProviderUserIdOnly();
        var request = ValidRequest();

        await useCase.ExecuteAsync(request);

        fixture.MockRepository.Verify(
            repo => repo.AddAsync(It.Is<Tenant>(t => t.CreatedBy == "keycloak-user-id"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Use_System_When_User_Context_Is_Empty()
    {
        var useCase = fixture.CreateUseCaseWithSystemUser();
        var request = ValidRequest();

        await useCase.ExecuteAsync(request);

        fixture.MockRepository.Verify(
            repo => repo.AddAsync(It.Is<Tenant>(t => t.CreatedBy == "system"), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
