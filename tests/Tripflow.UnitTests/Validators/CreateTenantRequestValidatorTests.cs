using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.UnitTests.Fixtures.Validators;

namespace Tripflow.UnitTests.Validators;

public class CreateTenantRequestValidatorTests(CreateTenantRequestValidatorFixture fixture) : IClassFixture<CreateTenantRequestValidatorFixture>
{
    private static CreateTenantRequest ValidRequest() => new(
        "Razão Social LTDA",
        "Nome Fantasia",
        "12345678000199",
        "tenant@test.com",
        "11999999999");

    [Fact]
    public async Task Validate_Should_Return_No_Errors_When_Request_Is_Valid()
    {
        var validator = fixture.CreateValidatorForNoDuplicates();
        var request = ValidRequest();

        var result = await validator.ValidateAsync(request);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_LegalName_Is_Empty()
    {
        var validator = fixture.CreateValidatorForNoDuplicates();
        var request = new CreateTenantRequest("", "Nome Fantasia", null, null, null);

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "A razão social é obrigatória.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_LegalName_Exceeds_200_Characters()
    {
        var validator = fixture.CreateValidatorForNoDuplicates();
        var request = new CreateTenantRequest(new string('A', 201), "Nome Fantasia", null, null, null);

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "A razão social deve ter no máximo 200 caracteres.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_TradeName_Is_Empty()
    {
        var validator = fixture.CreateValidatorForNoDuplicates();
        var request = new CreateTenantRequest("Razão Social LTDA", "", null, null, null);

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O nome fantasia é obrigatório.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_TradeName_Exceeds_200_Characters()
    {
        var validator = fixture.CreateValidatorForNoDuplicates();
        var request = new CreateTenantRequest("Razão Social LTDA", new string('A', 201), null, null, null);

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O nome fantasia deve ter no máximo 200 caracteres.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_DocumentNumber_Exceeds_20_Characters()
    {
        var validator = fixture.CreateValidatorForNoDuplicates();
        var request = new CreateTenantRequest("Razão Social LTDA", "Nome Fantasia", new string('1', 21), null, null);

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O documento deve ter no máximo 20 caracteres.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_DocumentNumber_Already_Exists()
    {
        var validator = fixture.CreateValidatorForDocumentExists();
        var request = ValidRequest();

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Já existe um tenant cadastrado com este documento.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Email_Is_Invalid()
    {
        var validator = fixture.CreateValidatorForNoDuplicates();
        var request = new CreateTenantRequest("Razão Social LTDA", "Nome Fantasia", null, "invalid-email", null);

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O e-mail informado é inválido.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Email_Exceeds_256_Characters()
    {
        var validator = fixture.CreateValidatorForNoDuplicates();
        var request = new CreateTenantRequest("Razão Social LTDA", "Nome Fantasia", null, $"{new string('a', 250)}@test.com", null);

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O e-mail deve ter no máximo 256 caracteres.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Email_Already_Exists()
    {
        var validator = fixture.CreateValidatorForEmailExists();
        var request = ValidRequest();

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Já existe um tenant cadastrado com este e-mail.");
    }

    [Fact]
    public async Task Validate_Should_Return_Error_When_Phone_Exceeds_20_Characters()
    {
        var validator = fixture.CreateValidatorForNoDuplicates();
        var request = new CreateTenantRequest("Razão Social LTDA", "Nome Fantasia", null, null, new string('1', 21));

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O telefone deve ter no máximo 20 caracteres.");
    }

    [Fact]
    public async Task Validate_Should_Return_Multiple_Errors_When_Multiple_Fields_Are_Invalid()
    {
        var validator = fixture.CreateValidatorForNoDuplicates();
        var request = new CreateTenantRequest("", "", null, "invalid-email", new string('1', 21));

        var result = await validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 3);
    }
}
