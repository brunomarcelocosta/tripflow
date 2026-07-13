using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Domain.Enums;
using Tripflow.UnitTests.Fixtures.UseCases;

namespace Tripflow.UnitTests.UseCases;

public class UpdateCustomerUseCaseTests(UpdateCustomerUseCaseFixture fixture)
    : IClassFixture<UpdateCustomerUseCaseFixture>
{
    private static UpdateCustomerRequest ValidRequest() => new(
        CustomerType.Person,
        "João Atualizado",
        "12345678900",
        "joao@teste.com",
        "11999999999",
        new DateOnly(1990, 1, 1),
        "Notas",
        CustomerStatus.Active);

    [Fact]
    public async Task ExecuteAsync_Should_Update_When_Found()
    {
        var (useCase, customer) = fixture.CreateForSuccess();

        var result = await useCase.ExecuteAsync(customer.Id, ValidRequest());

        Assert.True(result.Success);
        Assert.Equal("João Atualizado", result.Data!.FullName);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Failure_When_NotFound()
    {
        var useCase = fixture.CreateForNotFound();
        var result = await useCase.ExecuteAsync(Guid.NewGuid(), ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("Cliente não encontrado.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Block_CrossTenant_Access()
    {
        var (useCase, customer) = fixture.CreateForCrossTenant();
        var result = await useCase.ExecuteAsync(customer.Id, ValidRequest());

        Assert.False(result.Success);
        Assert.Equal("Cliente não encontrado.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Fail_On_Duplicate_Document()
    {
        var (useCase, customer) = fixture.CreateForDuplicateDocument();
        var result = await useCase.ExecuteAsync(customer.Id, ValidRequest());

        Assert.False(result.Success);
        Assert.Contains("documento", result.Error!);
    }
}
