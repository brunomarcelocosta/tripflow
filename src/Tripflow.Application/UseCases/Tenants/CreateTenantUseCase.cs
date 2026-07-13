using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.UseCases.Tenants.Interfaces;
using Tripflow.Domain.Entities.Tenants;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Tenants;

public class CreateTenantUseCase(
    ITenantRepository repository,
    ITenantRoleProvisioningService tenantRoleProvisioningService,
    IUserPermissionService userPermissionService,
    IValidator<CreateTenantRequest> validator,
    IUserContext userContext,
    ILogger<CreateTenantUseCase> logger) : ICreateTenantUseCase
{
    public string ClassName = nameof(CreateTenantUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<Guid?>> ExecuteAsync(CreateTenantRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<Guid?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.TenantsWrite,
                cancellationToken))
        {
            return Result<Guid?>.Forbidden();
        }

        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("{ClassName} | {Method} | Erro de validação | UserId={UserId}", ClassName, Method, createdBy);

            var firstError = validationResult.Errors.FirstOrDefault();

            return Result<Guid?>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            var Tenant = new Tenant(
                request.LegalName,
                request.TradeName,
                request.DocumentNumber,
                request.Email,
                request.Phone,
                TenantStatus.Active,
                createdBy);

            await repository.AddAsync(Tenant, cancellationToken);

            await tenantRoleProvisioningService.ProvisionDefaultRolesAsync(Tenant.Id, cancellationToken);

            await repository.CommitTransactionAsync(cancellationToken);

            logger.LogInformation("{ClassName} | {Method} | Tenant cadastrado com sucesso | {Id}", ClassName, Method, Tenant.Id);

            return Result<Guid?>.Ok(Tenant.Id);
        }
        catch (Exception ex)
        {
            logger.LogError("{ClassName} | {Method} | Erro inesperado ao cadastrar tenant | UserId={UserId} | {Message}",
                ClassName, Method, createdBy, ex.Message);

            await repository.RollbackTransactionAsync(cancellationToken);

            return Result<Guid?>.Failure("Erro inesperado ao criar tenant.");
        }
    }
}