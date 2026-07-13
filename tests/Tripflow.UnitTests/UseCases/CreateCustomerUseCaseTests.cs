using Moq;
using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Enums;
using Tripflow.UnitTests.Fixtures.UseCases;

namespace Tripflow.UnitTests.UseCases;

public class CreateCustomerUseCaseTests(CreateCustomerUseCaseFixture fixture)
    : IClassFixture<CreateCustomerUseCaseFixture>
{
    private static CreateCustomerRequest ValidRequest() => new(
        CustomerType.Person,
        "João da Silva",
        "12345678900",
        "joao@teste.com",
        "11999999999",
        new DateOnly(1990, 1, 1),
        "Notas");

    [Fact]
    public async Task ExecuteAsync_Should_CreateCustomer_With_TenantId()
    {
        var useCase = fixture.CreateForSuccess();

        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(fixture.CurrentTenantId, result.Data!.TenantId);
        Assert.Equal(CustomerStatus.Active, result.Data.Status);

        fixture.MockRepository.Verify(
            r => r.AddAsync(It.Is<Customer>(c => c.TenantId == fixture.CurrentTenantId), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Forbidden_When_Unauthenticated()
    {
        var useCase = fixture.CreateForUnauthenticated();
        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Forbidden_When_NoTenant()
    {
        var useCase = fixture.CreateForNoTenant();
        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Forbidden_When_NoPermission()
    {
        var useCase = fixture.CreateForNoPermission();
        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Validation_Fails()
    {
        var useCase = fixture.CreateForValidationFailure("FullName é obrigatório.");
        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("FullName é obrigatório.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_DocumentNumber_Duplicate()
    {
        var useCase = fixture.CreateForDuplicateDocument();
        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.Contains("documento", result.Error!);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Email_Duplicate()
    {
        var useCase = fixture.CreateForDuplicateEmail();
        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.Contains("e-mail", result.Error!);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_Repository_Throws()
    {
        var useCase = fixture.CreateForRepositoryError();
        var result = await useCase.ExecuteAsync(ValidRequest());

        Assert.False(result.Success);
        Assert.Contains("inesperado", result.Error!);
    }
}
