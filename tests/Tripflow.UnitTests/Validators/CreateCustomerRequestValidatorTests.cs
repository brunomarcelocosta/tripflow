using Tripflow.Application.DTOs.Requests.Customers;
using Tripflow.Application.Validators.Customers;
using Tripflow.Domain.Enums;

namespace Tripflow.UnitTests.Validators;

public class CreateCustomerRequestValidatorTests
{
    private readonly CreateCustomerRequestValidator _validator = new();

    private static CreateCustomerRequest Valid() => new(
        CustomerType.Person,
        "João da Silva",
        "12345678900",
        "joao@teste.com",
        "11999999999",
        new DateOnly(1990, 1, 1),
        "Notas");

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
    public async Task TooLongFullName_Should_Fail()
    {
        var result = await _validator.ValidateAsync(Valid() with { FullName = new string('a', 201) });
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task InvalidEmail_Should_Fail()
    {
        var result = await _validator.ValidateAsync(Valid() with { Email = "not-an-email" });
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task FutureBirthDate_Should_Fail()
    {
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1);
        var result = await _validator.ValidateAsync(Valid() with { BirthDate = futureDate });
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task TooLongNotes_Should_Fail()
    {
        var result = await _validator.ValidateAsync(Valid() with { Notes = new string('x', 4001) });
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task TooLongPhone_Should_Fail()
    {
        var result = await _validator.ValidateAsync(Valid() with { Phone = new string('9', 51) });
        Assert.False(result.IsValid);
    }
}
