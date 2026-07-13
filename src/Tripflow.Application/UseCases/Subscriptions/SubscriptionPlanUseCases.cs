using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.Builders;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.DTOs.Responses;
using Tripflow.Application.DTOs.Responses.Subscriptions;
using Tripflow.Application.Helpers;
using Tripflow.Application.UseCases.Subscriptions.Interfaces;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Subscriptions;

public sealed class GetSubscriptionPlansUseCase(
    ISubscriptionPlanRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    IValidator<SubscriptionPlanFilterRequest> validator) : IGetSubscriptionPlansUseCase
{
    public async Task<Result<PagedResponse<SubscriptionPlanResponse>>> ExecuteAsync(SubscriptionPlanFilterRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<PagedResponse<SubscriptionPlanResponse>>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsRead, cancellationToken))
            return Result<PagedResponse<SubscriptionPlanResponse>>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<PagedResponse<SubscriptionPlanResponse>>.Failure(validation.Errors.First().ErrorMessage);

        var paged = await repository.GetPagedAsync(
            request.ToExpression(),
            request.Page,
            request.PageSize,
            SubscriptionPlanOrderByHelper.Build(request.SortBy),
            request.SortDesc,
            x => x.Features);

        var mapped = paged.Items.Select(MapPlan).ToList();
        return Result<PagedResponse<SubscriptionPlanResponse>>.Ok(new PagedResponse<SubscriptionPlanResponse>(
            mapped,
            paged.Page,
            paged.PageSize,
            paged.TotalItems,
            paged.TotalPages));
    }

    private static SubscriptionPlanResponse MapPlan(SubscriptionPlan plan)
        => new()
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            MonthlyPrice = plan.MonthlyPrice,
            AnnualPrice = plan.AnnualPrice,
            MaxUsers = plan.MaxUsers,
            MaxQuotesPerMonth = plan.MaxQuotesPerMonth,
            Status = plan.Status,
            CreatedAtUtc = plan.CreatedAtUtc,
            UpdatedAtUtc = plan.UpdatedAtUtc,
            Features = plan.Features.Select(MapFeature).ToList()
        };

    private static PlanFeatureResponse MapFeature(PlanFeature feature)
        => new()
        {
            Id = feature.Id,
            SubscriptionPlanId = feature.SubscriptionPlanId,
            FeatureCode = feature.FeatureCode,
            LimitValue = feature.LimitValue,
            Enabled = feature.Enabled
        };
}

public sealed class GetSubscriptionPlanByIdUseCase(
    ISubscriptionPlanRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService) : IGetSubscriptionPlanByIdUseCase
{
    public async Task<Result<SubscriptionPlanResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<SubscriptionPlanResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsRead, cancellationToken))
            return Result<SubscriptionPlanResponse?>.Forbidden();

        var plan = await repository.GetByIdWithFeaturesAsync(id, cancellationToken);
        if (plan is null)
            return Result<SubscriptionPlanResponse?>.Ok(null);

        return Result<SubscriptionPlanResponse?>.Ok(new SubscriptionPlanResponse
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            MonthlyPrice = plan.MonthlyPrice,
            AnnualPrice = plan.AnnualPrice,
            MaxUsers = plan.MaxUsers,
            MaxQuotesPerMonth = plan.MaxQuotesPerMonth,
            Status = plan.Status,
            CreatedAtUtc = plan.CreatedAtUtc,
            UpdatedAtUtc = plan.UpdatedAtUtc,
            Features = plan.Features.Select(f => new PlanFeatureResponse
            {
                Id = f.Id,
                SubscriptionPlanId = f.SubscriptionPlanId,
                FeatureCode = f.FeatureCode,
                LimitValue = f.LimitValue,
                Enabled = f.Enabled
            }).ToList()
        });
    }
}

public sealed class CreateSubscriptionPlanUseCase(
    ISubscriptionPlanRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    IValidator<CreateSubscriptionPlanRequest> validator,
    ILogger<CreateSubscriptionPlanUseCase> logger) : ICreateSubscriptionPlanUseCase
{
    public async Task<Result<SubscriptionPlanResponse?>> ExecuteAsync(CreateSubscriptionPlanRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<SubscriptionPlanResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<SubscriptionPlanResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<SubscriptionPlanResponse?>.Failure(validation.Errors.First().ErrorMessage);

        var createdBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";
        try
        {
            var entity = new SubscriptionPlan(
                request.Name.Trim(),
                request.Description?.Trim(),
                request.MonthlyPrice,
                request.AnnualPrice,
                request.MaxUsers,
                request.MaxQuotesPerMonth,
                request.Status,
                createdBy);

            await repository.AddAsync(entity, cancellationToken);

            return Result<SubscriptionPlanResponse?>.Ok(new SubscriptionPlanResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                MonthlyPrice = entity.MonthlyPrice,
                AnnualPrice = entity.AnnualPrice,
                MaxUsers = entity.MaxUsers,
                MaxQuotesPerMonth = entity.MaxQuotesPerMonth,
                Status = entity.Status,
                CreatedAtUtc = entity.CreatedAtUtc,
                UpdatedAtUtc = entity.UpdatedAtUtc
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CreateSubscriptionPlanUseCase | Erro ao criar plano.");
            return Result<SubscriptionPlanResponse?>.Failure("Erro inesperado ao criar plano de assinatura.");
        }
    }
}

public sealed class UpdateSubscriptionPlanUseCase(
    ISubscriptionPlanRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    IValidator<UpdateSubscriptionPlanRequest> validator,
    ILogger<UpdateSubscriptionPlanUseCase> logger) : IUpdateSubscriptionPlanUseCase
{
    public async Task<Result<SubscriptionPlanResponse?>> ExecuteAsync(Guid id, UpdateSubscriptionPlanRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<SubscriptionPlanResponse?>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<SubscriptionPlanResponse?>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<SubscriptionPlanResponse?>.Failure(validation.Errors.First().ErrorMessage);

        if (await repository.ExistsByNameExceptIdAsync(request.Name.Trim(), id, cancellationToken))
            return Result<SubscriptionPlanResponse?>.Failure("Já existe um plano com este nome.");

        var entity = await repository.GetTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
            return Result<SubscriptionPlanResponse?>.Failure("Plano não encontrado.");

        var updatedBy = userContext.Email ?? userContext.IdentityProviderUserId ?? "system";

        try
        {
            entity.Update(
                request.Name.Trim(),
                request.Description?.Trim(),
                request.MonthlyPrice,
                request.AnnualPrice,
                request.MaxUsers,
                request.MaxQuotesPerMonth,
                request.Status,
                updatedBy);

            await repository.UpdateAsync(entity, cancellationToken);

            return Result<SubscriptionPlanResponse?>.Ok(new SubscriptionPlanResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                MonthlyPrice = entity.MonthlyPrice,
                AnnualPrice = entity.AnnualPrice,
                MaxUsers = entity.MaxUsers,
                MaxQuotesPerMonth = entity.MaxQuotesPerMonth,
                Status = entity.Status,
                CreatedAtUtc = entity.CreatedAtUtc,
                UpdatedAtUtc = entity.UpdatedAtUtc
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateSubscriptionPlanUseCase | Erro ao atualizar plano.");
            return Result<SubscriptionPlanResponse?>.Failure("Erro inesperado ao atualizar plano de assinatura.");
        }
    }
}

public sealed class DeleteSubscriptionPlanUseCase(
    ISubscriptionPlanRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    ILogger<DeleteSubscriptionPlanUseCase> logger) : IDeleteSubscriptionPlanUseCase
{
    public async Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<bool>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<bool>.Forbidden();

        var entity = await repository.GetTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
            return Result<bool>.Failure("Plano não encontrado.");

        if (await repository.HasActiveSubscriptionsAsync(id, cancellationToken))
            return Result<bool>.Failure("Não é possível remover um plano com assinaturas ativas.");

        try
        {
            entity.SetDelete(userContext.Email ?? userContext.IdentityProviderUserId ?? "system");
            await repository.UpdateAsync(entity, cancellationToken);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteSubscriptionPlanUseCase | Erro ao remover plano.");
            return Result<bool>.Failure("Erro inesperado ao remover plano de assinatura.");
        }
    }
}

public sealed class ActivateSubscriptionPlanUseCase(
    ISubscriptionPlanRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    ILogger<ActivateSubscriptionPlanUseCase> logger) : IActivateSubscriptionPlanUseCase
{
    public async Task<Result<SubscriptionPlanResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await UpdateStatusAsync(id, cancellationToken, "activate", (plan, user) => plan.Activate(user));
    }

    private async Task<Result<SubscriptionPlanResponse?>> UpdateStatusAsync(Guid id, CancellationToken cancellationToken, string action, Action<SubscriptionPlan, string> apply)
    {
        if (!userContext.IsAuthenticated)
            return Result<SubscriptionPlanResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<SubscriptionPlanResponse?>.Forbidden();

        var entity = await repository.GetTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
            return Result<SubscriptionPlanResponse?>.Failure("Plano não encontrado.");

        try
        {
            apply(entity, userContext.Email ?? userContext.IdentityProviderUserId ?? "system");
            await repository.UpdateAsync(entity, cancellationToken);
            return Result<SubscriptionPlanResponse?>.Ok(new SubscriptionPlanResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                MonthlyPrice = entity.MonthlyPrice,
                AnnualPrice = entity.AnnualPrice,
                MaxUsers = entity.MaxUsers,
                MaxQuotesPerMonth = entity.MaxQuotesPerMonth,
                Status = entity.Status,
                CreatedAtUtc = entity.CreatedAtUtc,
                UpdatedAtUtc = entity.UpdatedAtUtc
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ActivateSubscriptionPlanUseCase | Erro ao {Action}.", action);
            return Result<SubscriptionPlanResponse?>.Failure("Erro inesperado ao atualizar status do plano.");
        }
    }
}

public sealed class InactivateSubscriptionPlanUseCase(
    ISubscriptionPlanRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    ILogger<InactivateSubscriptionPlanUseCase> logger) : IInactivateSubscriptionPlanUseCase
{
    public async Task<Result<SubscriptionPlanResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<SubscriptionPlanResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<SubscriptionPlanResponse?>.Forbidden();

        var entity = await repository.GetTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
            return Result<SubscriptionPlanResponse?>.Failure("Plano não encontrado.");

        try
        {
            entity.Inactivate(userContext.Email ?? userContext.IdentityProviderUserId ?? "system");
            await repository.UpdateAsync(entity, cancellationToken);
            return Result<SubscriptionPlanResponse?>.Ok(new SubscriptionPlanResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                MonthlyPrice = entity.MonthlyPrice,
                AnnualPrice = entity.AnnualPrice,
                MaxUsers = entity.MaxUsers,
                MaxQuotesPerMonth = entity.MaxQuotesPerMonth,
                Status = entity.Status,
                CreatedAtUtc = entity.CreatedAtUtc,
                UpdatedAtUtc = entity.UpdatedAtUtc
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "InactivateSubscriptionPlanUseCase | Erro ao inativar.");
            return Result<SubscriptionPlanResponse?>.Failure("Erro inesperado ao inativar plano.");
        }
    }
}

public sealed class DeprecateSubscriptionPlanUseCase(
    ISubscriptionPlanRepository repository,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    ILogger<DeprecateSubscriptionPlanUseCase> logger) : IDeprecateSubscriptionPlanUseCase
{
    public async Task<Result<SubscriptionPlanResponse?>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<SubscriptionPlanResponse?>.Forbidden();
        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<SubscriptionPlanResponse?>.Forbidden();

        var entity = await repository.GetTrackedByIdAsync(id, cancellationToken);
        if (entity is null)
            return Result<SubscriptionPlanResponse?>.Failure("Plano não encontrado.");

        try
        {
            entity.Deprecate(userContext.Email ?? userContext.IdentityProviderUserId ?? "system");
            await repository.UpdateAsync(entity, cancellationToken);
            return Result<SubscriptionPlanResponse?>.Ok(new SubscriptionPlanResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                MonthlyPrice = entity.MonthlyPrice,
                AnnualPrice = entity.AnnualPrice,
                MaxUsers = entity.MaxUsers,
                MaxQuotesPerMonth = entity.MaxQuotesPerMonth,
                Status = entity.Status,
                CreatedAtUtc = entity.CreatedAtUtc,
                UpdatedAtUtc = entity.UpdatedAtUtc
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeprecateSubscriptionPlanUseCase | Erro ao descontinuar.");
            return Result<SubscriptionPlanResponse?>.Failure("Erro inesperado ao descontinuar plano.");
        }
    }
}
