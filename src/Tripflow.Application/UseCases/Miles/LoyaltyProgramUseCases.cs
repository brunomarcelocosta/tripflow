using FluentValidation;
using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Miles;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Miles;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Miles.Interfaces;
using Tripflow.Domain.Entities.Miles;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Miles;

public sealed class GetLoyaltyProgramsUseCase(
    ILoyaltyProgramRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService) : IGetLoyaltyProgramsUseCase
{
    public async Task<Result<PagedResponse<LoyaltyProgramResponse>>> ExecuteAsync(LoyaltyProgramFilterRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<LoyaltyProgramResponse>>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesRead, cancellationToken))
            return Result<PagedResponse<LoyaltyProgramResponse>>.Forbidden();

        var filter = request.ToExpression();
        var orderBy = LoyaltyProgramOrderByHelper.Build(request.SortBy);
        var paged = await repository.GetPagedAsync(filter, request.Page, request.PageSize, orderBy, request.SortDesc);
        var items = paged.Items.Select(ToResponse).ToList();
        return Result<PagedResponse<LoyaltyProgramResponse>>.Ok(
            new PagedResponse<LoyaltyProgramResponse>(items, paged.Page, paged.PageSize, paged.TotalItems, paged.TotalPages));
    }

    private static LoyaltyProgramResponse ToResponse(LoyaltyProgram item)
        => new(item.Id, item.Name, item.Country, item.AirlineName, item.Status, item.CreatedAtUtc, item.UpdatedAtUtc);
}

public sealed class GetLoyaltyProgramByIdUseCase(
    ILoyaltyProgramRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService) : IGetLoyaltyProgramByIdUseCase
{
    public async Task<Result<LoyaltyProgramResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<LoyaltyProgramResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.MilesRead, cancellationToken))
            return Result<LoyaltyProgramResponse?>.Forbidden();

        var program = await repository.GetByIdAsync(id);
        if (program is null)
            return Result<LoyaltyProgramResponse?>.Failure("Programa de fidelidade não encontrado.");

        return Result<LoyaltyProgramResponse?>.Ok(
            new LoyaltyProgramResponse(program.Id, program.Name, program.Country, program.AirlineName, program.Status, program.CreatedAtUtc, program.UpdatedAtUtc));
    }
}

public sealed class CreateLoyaltyProgramUseCase(
    ILoyaltyProgramRepository repository,
    IValidator<CreateLoyaltyProgramRequest> validator,
    IUserContext userContext,
    IUserPermissionService userPermissionService) : ICreateLoyaltyProgramUseCase
{
    public async Task<Result<LoyaltyProgramResponse?>> ExecuteAsync(CreateLoyaltyProgramRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<LoyaltyProgramResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<LoyaltyProgramResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<LoyaltyProgramResponse?>.Failure(validation.Errors.First().ErrorMessage);

        if (await repository.ExistsByNameAsync(request.Name.Trim(), cancellationToken))
            return Result<LoyaltyProgramResponse?>.Failure("Já existe um programa de fidelidade com este nome.");

        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        var entity = new LoyaltyProgram(
            request.Name.Trim(),
            request.Country?.Trim(),
            request.AirlineName?.Trim(),
            request.Status ?? LoyaltyProgramStatus.Active,
            createdBy);

        await repository.AddAsync(entity, cancellationToken);
        return Result<LoyaltyProgramResponse?>.Ok(
            new LoyaltyProgramResponse(entity.Id, entity.Name, entity.Country, entity.AirlineName, entity.Status, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }
}

public sealed class UpdateLoyaltyProgramUseCase(
    ILoyaltyProgramRepository repository,
    IValidator<UpdateLoyaltyProgramRequest> validator,
    IUserContext userContext,
    IUserPermissionService userPermissionService) : IUpdateLoyaltyProgramUseCase
{
    public async Task<Result<LoyaltyProgramResponse?>> ExecuteAsync(Guid id, UpdateLoyaltyProgramRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<LoyaltyProgramResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<LoyaltyProgramResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<LoyaltyProgramResponse?>.Failure(validation.Errors.First().ErrorMessage);

        if (await repository.ExistsByNameExceptIdAsync(request.Name.Trim(), id, cancellationToken))
            return Result<LoyaltyProgramResponse?>.Failure("Já existe um programa de fidelidade com este nome.");

        var program = await repository.GetTrackedByIdAsync(id, cancellationToken);
        if (program is null)
            return Result<LoyaltyProgramResponse?>.Failure("Programa de fidelidade não encontrado.");

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        program.Update(request.Name.Trim(), request.Country?.Trim(), request.AirlineName?.Trim(), request.Status, updatedBy);
        await repository.UpdateAsync(program, cancellationToken);

        return Result<LoyaltyProgramResponse?>.Ok(
            new LoyaltyProgramResponse(program.Id, program.Name, program.Country, program.AirlineName, program.Status, program.CreatedAtUtc, program.UpdatedAtUtc));
    }
}

public sealed class ActivateLoyaltyProgramUseCase(
    ILoyaltyProgramRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService) : IActivateLoyaltyProgramUseCase
{
    public async Task<Result<LoyaltyProgramResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<LoyaltyProgramResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<LoyaltyProgramResponse?>.Forbidden();

        var program = await repository.GetTrackedByIdAsync(id, cancellationToken);
        if (program is null)
            return Result<LoyaltyProgramResponse?>.Failure("Programa de fidelidade não encontrado.");

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        program.Activate(updatedBy);
        await repository.UpdateAsync(program, cancellationToken);

        return Result<LoyaltyProgramResponse?>.Ok(
            new LoyaltyProgramResponse(program.Id, program.Name, program.Country, program.AirlineName, program.Status, program.CreatedAtUtc, program.UpdatedAtUtc));
    }
}

public sealed class InactivateLoyaltyProgramUseCase(
    ILoyaltyProgramRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService) : IInactivateLoyaltyProgramUseCase
{
    public async Task<Result<LoyaltyProgramResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<LoyaltyProgramResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<LoyaltyProgramResponse?>.Forbidden();

        var program = await repository.GetTrackedByIdAsync(id, cancellationToken);
        if (program is null)
            return Result<LoyaltyProgramResponse?>.Failure("Programa de fidelidade não encontrado.");

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        program.Inactivate(updatedBy);
        await repository.UpdateAsync(program, cancellationToken);

        return Result<LoyaltyProgramResponse?>.Ok(
            new LoyaltyProgramResponse(program.Id, program.Name, program.Country, program.AirlineName, program.Status, program.CreatedAtUtc, program.UpdatedAtUtc));
    }
}

public sealed class DeleteLoyaltyProgramUseCase(
    ILoyaltyProgramRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService) : IDeleteLoyaltyProgramUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<bool>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<bool>.Forbidden();

        var program = await repository.GetTrackedByIdAsync(id, cancellationToken);
        if (program is null)
            return Result<bool>.Failure("Programa de fidelidade não encontrado.");

        if (await repository.HasLinkedAccountsAsync(id, cancellationToken))
            return Result<bool>.Failure("Não é possível remover o programa porque existem contas vinculadas.");

        var deletedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        program.SetDelete(deletedBy);
        await repository.UpdateAsync(program, cancellationToken);
        return Result<bool>.Ok(true);
    }
}
