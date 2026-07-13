using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Application.Validators.Customers;
using Tripflow.Domain.Enums;

namespace Tripflow.UnitTests.Validators;

public class UpdateCustomerRequestValidatorTests
{
    private readonly UpdateCustomerRequestValidator _validator = new();

    private static UpdateCustomerRequest Valid() => new(
        CustomerType.Person,
        "João da Silva",
        "12345678900",
        "joao@teste.com",
        "11999999999",
        new DateOnly(1990, 1, 1),
        "Notas",
        CustomerStatus.Active);

    [Fact]
    public async Task Valid_Should_Pass()
    {
        var result = await _validator.ValidateAsync(Valid());
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task EmptyFullName_Should_Fail()
    {
        var result = await _validator.ValidateAsync(Valid() with { FullName = "" });
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task InvalidStatus_Should_Fail()
    {
        var result = await _validator.ValidateAsync(Valid() with { Status = (CustomerStatus)99 });
        Assert.False(result.IsValid);
    }
}
