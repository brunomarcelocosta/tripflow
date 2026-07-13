using FluentValidation;
using Tripflow.Application.DTOs.Requests.Tenants;

namespace Tripflow.Application.Validators.Tenants;

public sealed class UploadTenantLogoRequestValidator : AbstractValidator<UploadTenantLogoRequest>
{
    private const long MaxLogoSizeBytes = 2 * 1024 * 1024;

    private static readonly string[] AllowedExtensions =
    [
        ".png", ".jpg", ".jpeg", ".webp", ".svg"
    ];

    public UploadTenantLogoRequestValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("Nome do arquivo é obrigatório.")
            .Must(HasAllowedExtension!)
            .WithMessage("Extensão do arquivo não permitida. Use .png, .jpg, .jpeg, .webp ou .svg.");

        RuleFor(x => x.SizeBytes)
            .GreaterThan(0)
            .WithMessage("O arquivo enviado está vazio.")
            .LessThanOrEqualTo(MaxLogoSizeBytes)
            .WithMessage("O arquivo deve ter no máximo 2MB.");

        RuleFor(x => x.Content)
            .NotNull()
            .WithMessage("Conteúdo do arquivo é obrigatório.");
    }

    private static bool HasAllowedExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName);

        if (string.IsNullOrWhiteSpace(extension))
            return false;

        return AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }
}
