using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.UseCases.Tenants.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.API.Controllers.Tenants;

[ApiController]
[Route("api/tenant-branding")]
[Authorize]
public sealed class TenantBrandingController(
    IGetCurrentTenantBrandingUseCase getCurrentBrandingUseCase,
    IUpdateTenantBrandingUseCase updateBrandingUseCase,
    IUploadTenantLogoUseCase uploadLogoUseCase) : ControllerBase
{
    [HttpGet("current")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsRead)]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var result = await getCurrentBrandingUseCase.ExecuteAsync(cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("current")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    public async Task<IActionResult> UpdateCurrent(
        [FromBody] UpdateTenantBrandingRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateBrandingUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("current/logo")]
    [RequirePermission(TripflowDbSeedData.Permissions.SettingsManage)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadCurrentLogo(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null)
            return BadRequest(new { success = false, error = "Arquivo é obrigatório." });

        await using var stream = file.OpenReadStream();

        var request = new UploadTenantLogoRequest
        {
            Content = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            SizeBytes = file.Length
        };

        var result = await uploadLogoUseCase.ExecuteAsync(request, cancellationToken);

        if (result.IsForbidden)
            return Forbid();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
