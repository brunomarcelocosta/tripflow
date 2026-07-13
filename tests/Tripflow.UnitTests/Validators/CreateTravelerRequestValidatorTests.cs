using Tripflow.Application.DTOs.Requests.Travelers;
using Tripflow.Application.Validators.Travelers;

namespace Tripflow.UnitTests.Validators;

public class CreateTravelerRequestValidatorTests
{
    private readonly CreateTravelerRequestValidator _validator = new();

    private static CreateTravelerRequest Valid() => new(
        "Maria Souza",
        new DateOnly(1995, 5, 5),
        "Brasileira",
        "11122233344",
        "BR123456",
        new DateOnly(2030, 12, 31),
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
    public async Task TooLongPassportNumber_Should_Fail()
    {
        var result = await _validator.ValidateAsync(Valid() with { PassportNumber = new string('A', 51) });
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task TooLongNationality_Should_Fail()
    {
        var result = await _validator.ValidateAsync(Valid() with { Nationality = new string('a', 101) });
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task FutureBirthDate_Should_Fail()
    {
        var future = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1);
        var result = await _validator.ValidateAsync(Valid() with { BirthDate = future });
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ExpiredPassportDate_Should_Pass()
    {
        var expired = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1);
        var result = await _validator.ValidateAsync(Valid() with { PassportExpirationDate = expired });
        Assert.True(result.IsValid);
    }
}
