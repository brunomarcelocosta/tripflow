using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.Validators.Tenants;

namespace Tripflow.UnitTests.Validators;

public class UpdateTenantBrandingRequestValidatorTests
{
    private static UpdateTenantBrandingRequest ValidRequest() => new()
    {
        PrimaryColor = "#000000",
        SecondaryColor = "#FFFFFF",
        TextColor = "#1A1A1A",
        ProposalFooter = "Rodapé padrão"
    };

    private static UpdateTenantBrandingRequestValidator BuildValidator() => new();

    [Fact]
    public async Task Validate_Should_Pass_For_Valid_Request()
    {
        var result = await BuildValidator().ValidateAsync(ValidRequest());

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Allow_All_Null_Values()
    {
        var request = new UpdateTenantBrandingRequest();

        var result = await BuildValidator().ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("000000")]
    [InlineData("#GGGGGG")]
    [InlineData("#1234")]
    [InlineData("invalid")]
    public async Task Validate_Should_Fail_When_PrimaryColor_Is_Not_Hex(string invalid)
    {
        var request = ValidRequest() with { PrimaryColor = invalid };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "A cor primária deve estar no formato HEX (ex.: #FFFFFF).");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_SecondaryColor_Is_Not_Hex()
    {
        var request = ValidRequest() with { SecondaryColor = "white" };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "A cor secundária deve estar no formato HEX (ex.: #FFFFFF).");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_TextColor_Is_Not_Hex()
    {
        var request = ValidRequest() with { TextColor = "azul" };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "A cor do texto deve estar no formato HEX (ex.: #FFFFFF).");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_ProposalFooter_Exceeds_4000_Chars()
    {
        var request = ValidRequest() with { ProposalFooter = new string('A', 4001) };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O rodapé da proposta deve ter no máximo 4000 caracteres.");
    }

    [Theory]
    [InlineData("#FFF")]
    [InlineData("#abcdef")]
    [InlineData("#0F0F0F")]
    public async Task Validate_Should_Accept_Valid_Hex_Variations(string hex)
    {
        var request = ValidRequest() with { PrimaryColor = hex, SecondaryColor = hex, TextColor = hex };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.True(result.IsValid);
    }
}
