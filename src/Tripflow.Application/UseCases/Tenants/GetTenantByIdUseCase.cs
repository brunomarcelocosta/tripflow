using AutoMapper;
using FluentValidation;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Tenants;
using Tripflow.Application.DTOs.Responses.Tenants;
using Tripflow.Application.UseCases.Tenants.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Tenants;

public class GetTenantByIdUseCase(
    ITenantRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    IValidator<GetTenantByIdRequest> validator,
    IMapper mapper) : IGetTenantByIdUseCase
{
    public async Task<Result<TenantResponse?>> ExecuteAsync(GetTenantByIdRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<TenantResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(
                TripflowDbSeedData.Permissions.TenantsRead,
                cancellationToken))
        {
            return Result<TenantResponse?>.Forbidden();
        }

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault();
            return Result<TenantResponse?>.Failure(firstError?.ErrorMessage ?? "Erro de validação.");
        }

        var entity = await repository.GetByIdAsync(request.Id);

        if (entity is null)
            return Result<TenantResponse?>.Failure("Tenant não encontrado.");

        var response = mapper.Map<TenantResponse>(entity);

        return Result<TenantResponse?>.Ok(response);
    }
}
