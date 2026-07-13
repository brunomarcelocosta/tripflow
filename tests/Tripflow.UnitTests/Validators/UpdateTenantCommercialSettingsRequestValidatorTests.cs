using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.Validators.Tenants;

namespace Tripflow.UnitTests.Validators;

public class UpdateTenantCommercialSettingsRequestValidatorTests
{
    private static UpdateTenantCommercialSettingsRequestValidator BuildValidator() => new();

    private static UpdateTenantCommercialSettingsRequest ValidRequest() => new()
    {
        CommercialEmail = "comercial@empresa.com",
        CommercialPhone = "11988887777",
        WhatsApp = "11988887777",
        Instagram = "@empresa",
        Website = "https://empresa.com",
        DefaultTerms = "Termos.",
        DefaultImportantNotes = "Notas.",
        DefaultProposalExpirationHours = 48
    };

    [Fact]
    public async Task Validate_Should_Pass_For_Valid_Request()
    {
        var result = await BuildValidator().ValidateAsync(ValidRequest());

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Allow_Empty_Optional_Fields()
    {
        var request = new UpdateTenantCommercialSettingsRequest { DefaultProposalExpirationHours = 24 };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_Should_Fail_When_Email_Is_Invalid()
    {
        var request = ValidRequest() with { CommercialEmail = "invalido" };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O e-mail comercial informado é inválido.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_Website_Is_Invalid()
    {
        var request = ValidRequest() with { Website = "nao-eh-uma-url" };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O site informado é inválido.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_DefaultTerms_Exceeds_8000_Chars()
    {
        var request = ValidRequest() with { DefaultTerms = new string('A', 8001) };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Os termos padrão devem ter no máximo 8000 caracteres.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_DefaultImportantNotes_Exceeds_8000_Chars()
    {
        var request = ValidRequest() with { DefaultImportantNotes = new string('A', 8001) };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "As observações importantes devem ter no máximo 8000 caracteres.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_DefaultProposalExpirationHours_Is_Zero()
    {
        var request = ValidRequest() with { DefaultProposalExpirationHours = 0 };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "A validade padrão da proposta deve ser maior que 0 horas.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_Phone_Exceeds_Limit()
    {
        var request = ValidRequest() with { CommercialPhone = new string('1', 51) };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O telefone comercial deve ter no máximo 50 caracteres.");
    }
}
