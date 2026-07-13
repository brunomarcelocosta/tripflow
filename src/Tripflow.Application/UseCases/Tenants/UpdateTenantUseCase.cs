using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.DTOs.Responses.Tenants;
using Tripflow.Application.UseCases.Tenants.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Tenants;

public class UpdateTenantUseCase(
    IValidator<UpdateTenantValidationRequest> validator,
    ITenantRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    IMapper mapper,
    ILogger<UpdateTenantUseCase> logger) : IUpdateTenantUseCase
{
    public string ClassName = nameof(UpdateTenantUseCase);
    public string Method = nameof(ExecuteAsync);

    public async Task<Result<TenantResponse?>> ExecuteAsync(Guid id, UpdateTenantRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<TenantResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.TenantsWrite,
                cancellationToken))
        {
            return Result<TenantResponse?>.Forbidden();
        }

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        var validationResult = await validator.ValidateAsync(
            UpdateTenantValidationRequest.From(id, request),
            cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("{ClassName} | {Method} | Erro de validação | Id={Id}", ClassName, Method, id);

            var firstError = validationResult.Errors.FirstOrDefault();

            return Result<TenantResponse?>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            var entity = await repository.GetByIdAsync(id);

            entity!.Update(request.LegalName, request.TradeName, request.DocumentNumber, request.Email, request.Phone, request.Status, updatedBy);

            await repository.UpdateAsync(entity, cancellationToken);

            logger.LogInformation("{ClassName} | {Method} | Tenant atualizado com sucesso | {Id}", ClassName, Method, id);

            var response = mapper.Map<TenantResponse>(entity);

            return Result<TenantResponse?>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError("{ClassName} | {Method} | Erro inesperado ao atualizado tenant | UserId={UserId} | {Message}",
                ClassName, Method, updatedBy, ex.Message);

            await repository.RollbackTransactionAsync(cancellationToken);

            return Result<TenantResponse>.Failure("Erro inesperado ao atualizar tenant.");
        }
    }
}