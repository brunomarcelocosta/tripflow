using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.UnitTests.Fixtures.Validators;

namespace Tripflow.UnitTests.Validators;

public class UpdateTenantRequestValidatorTests(UpdateTenantRequestValidatorFixture fixture) : IClassFixture<UpdateTenantRequestValidatorFixture>
{
    private static UpdateTenantValidationRequest ValidRequest() => new(
        Guid.NewGuid(),
        "Razão Social LTDA",
        "Nome Fantasia",
        "12345678000199",
        "tenant@test.com",
        "11999999999",
        TenantStatus.Active);

    [Fact]
    public async Task Validate_Should_Return_No_Errors_When_Request_Is_Valid()
    {
        var validator = fixture.CreateForSuccess();
        var request = ValidRequest();

        var result = await validator.ValidateAsync(request);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Id_Is_Empty()
    {
        var validator = fixture.CreateForSuccess();
        var request = ValidRequest() with { Id = Guid.Empty };

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O identificador do tenant é obrigatório.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Tenant_Not_Found()
    {
        var validator = fixture.CreateForIdNotExists();
        var request = ValidRequest();

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Tenant não encontrado.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_LegalName_Is_Empty()
    {
        var validator = fixture.CreateForSuccess();
        var request = ValidRequest() with { LegalName = "" };

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "A razão social é obrigatória.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_LegalName_Exceeds_200_Characters()
    {
        var validator = fixture.CreateForSuccess();
        var request = ValidRequest() with { LegalName = new string('A', 201) };

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "A razão social deve ter no máximo 200 caracteres.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_TradeName_Is_Empty()
    {
        var validator = fixture.CreateForSuccess();
        var request = ValidRequest() with { TradeName = "" };

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O nome fantasia é obrigatório.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_TradeName_Exceeds_200_Characters()
    {
        var validator = fixture.CreateForSuccess();
        var request = ValidRequest() with { TradeName = new string('A', 201) };

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O nome fantasia deve ter no máximo 200 caracteres.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Status_Is_Invalid()
    {
        var validator = fixture.CreateForSuccess();
        var request = ValidRequest() with { Status = (TenantStatus)999 };

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O status informado é inválido.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_DocumentNumber_Exceeds_20_Characters()
    {
        var validator = fixture.CreateForSuccess();
        var request = ValidRequest() with { DocumentNumber = new string('1', 21) };

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O documento deve ter no máximo 20 caracteres.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_DocumentNumber_Exists_In_Other_Tenant()
    {
        var validator = fixture.CreateForDocumentExistsInOther();
        var request = ValidRequest();

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Já existe outro tenant cadastrado com este documento.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Email_Is_Invalid()
    {
        var validator = fixture.CreateForSuccess();
        var request = ValidRequest() with { Email = "invalid-email" };

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O e-mail informado é inválido.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Email_Exceeds_256_Characters()
    {
        var validator = fixture.CreateForSuccess();
        var request = ValidRequest() with { Email = $"{new string('a', 250)}@test.com" };

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O e-mail deve ter no máximo 256 caracteres.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Email_Exists_In_Other_Tenant()
    {
        var validator = fixture.CreateForEmailExistsInOther();
        var request = ValidRequest();

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Já existe outro tenant cadastrado com este e-mail.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Phone_Exceeds_20_Characters()
    {
        var validator = fixture.CreateForSuccess();
        var request = ValidRequest() with { Phone = new string('1', 21) };

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O telefone deve ter no máximo 20 caracteres.");
    }
}
