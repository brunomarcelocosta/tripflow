using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.Validators.Tenants;

namespace Tripflow.UnitTests.Validators;

public class UploadTenantLogoRequestValidatorTests
{
    private static UploadTenantLogoRequestValidator BuildValidator() => new();

    private static UploadTenantLogoRequest ValidRequest() => new()
    {
        Content = new MemoryStream([0x1, 0x2, 0x3]),
        FileName = "logo.png",
        ContentType = "image/png",
        SizeBytes = 1024
    };

    [Fact]
    public async Task Validate_Should_Pass_For_Valid_Request()
    {
        var result = await BuildValidator().ValidateAsync(ValidRequest());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("logo.png")]
    [InlineData("LOGO.JPG")]
    [InlineData("imagem.jpeg")]
    [InlineData("foto.webp")]
    [InlineData("vector.svg")]
    public async Task Validate_Should_Accept_Allowed_Extensions(string fileName)
    {
        var request = ValidRequest() with { FileName = fileName };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("malicioso.exe")]
    [InlineData("arquivo.txt")]
    [InlineData("documento.pdf")]
    [InlineData("semextensao")]
    public async Task Validate_Should_Reject_Disallowed_Extensions(string fileName)
    {
        var request = ValidRequest() with { FileName = fileName };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Extensão do arquivo não permitida. Use .png, .jpg, .jpeg, .webp ou .svg.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_FileName_Is_Empty()
    {
        var request = ValidRequest() with { FileName = "" };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Nome do arquivo é obrigatório.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_Size_Is_Zero()
    {
        var request = ValidRequest() with { SizeBytes = 0 };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O arquivo enviado está vazio.");
    }

    [Fact]
    public async Task Validate_Should_Fail_When_Size_Exceeds_2MB()
    {
        var request = ValidRequest() with { SizeBytes = (2 * 1024 * 1024) + 1 };

        var result = await BuildValidator().ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O arquivo deve ter no máximo 2MB.");
    }
}
