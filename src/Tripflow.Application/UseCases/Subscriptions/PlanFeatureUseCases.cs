using FluentValidation;
using Microsoft.Extensions.Logging;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Requests.Subscriptions;
using Tripflow.Application.DTOs.Responses.Subscriptions;
using Tripflow.Application.UseCases.Subscriptions.Interfaces;
using Tripflow.Domain.Entities.Subscriptions;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Data.Seeds;

namespace Tripflow.Application.UseCases.Subscriptions;

public sealed class GetPlanFeaturesUseCase(
    ISubscriptionPlanRepository subscriptionPlanRepository,
    IPlanFeatureRepository planFeatureRepository,
    IUserContext userContext,
    IUserPermissionService userPermissionService) : IGetPlanFeaturesUseCase
{
    public async Task<Result<IEnumerable<PlanFeatureResponse>>> ExecuteAsync(Guid planId, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<IEnumerable<PlanFeatureResponse>>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsRead, cancellationToken))
            return Result<IEnumerable<PlanFeatureResponse>>.Forbidden();

        var plan = await subscriptionPlanRepository.GetTrackedByIdAsync(planId, cancellationToken);
        if (plan is null)
            return Result<IEnumerable<PlanFeatureResponse>>.Failure("Plano não encontrado.");

        var features = await planFeatureRepository.GetByPlanIdAsync(planId, cancellationToken);
        return Result<IEnumerable<PlanFeatureResponse>>.Ok(features.Select(Map).ToList());
    }

    private static PlanFeatureResponse Map(PlanFeature feature)
        => new()
        {
            Id = feature.Id,
            SubscriptionPlanId = feature.SubscriptionPlanId,
            FeatureCode = feature.FeatureCode,
            LimitValue = feature.LimitValue,
            Enabled = feature.Enabled
        };
}

public sealed class UpdatePlanFeaturesUseCase(
    ISubscriptionPlanRepository subscriptionPlanRepository,
    IPlanFeatureRepository planFeatureRepository,
    IUserContext userContext,
    IUserPermissionService userPermissionService,
    IValidator<UpdatePlanFeaturesRequest> validator,
    ILogger<UpdatePlanFeaturesUseCase> logger) : IUpdatePlanFeaturesUseCase
{
    public async Task<Result<IEnumerable<PlanFeatureResponse>>> ExecuteAsync(Guid planId, UpdatePlanFeaturesRequest request, CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<IEnumerable<PlanFeatureResponse>>.Forbidden();

        if (!await userPermissionService.HasPermissionAsync(TripflowDbSeedData.Permissions.SettingsManage, cancellationToken))
            return Result<IEnumerable<PlanFeatureResponse>>.Forbidden();

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Result<IEnumerable<PlanFeatureResponse>>.Failure(validation.Errors.First().ErrorMessage);

        var plan = await subscriptionPlanRepository.GetTrackedByIdAsync(planId, cancellationToken);
        if (plan is null)
            return Result<IEnumerable<PlanFeatureResponse>>.Failure("Plano não encontrado.");

        await planFeatureRepository.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (var item in request.Features)
            {
                var code = item.FeatureCode.Trim().ToLowerInvariant();
                var existing = await planFeatureRepository.GetByPlanAndCodeAsync(planId, code, cancellationToken);
                if (existing is null)
                {
                    await planFeatureRepository.AddAsync(new PlanFeature(planId, code, item.LimitValue, item.Enabled), cancellationToken);
                }
                else
                {
                    existing.Update(item.LimitValue, item.Enabled);
                    await planFeatureRepository.UpdateAsync(existing, cancellationToken);
                }
            }

            await planFeatureRepository.CommitTransactionAsync(cancellationToken);

            var features = await planFeatureRepository.GetByPlanIdAsync(planId, cancellationToken);
            return Result<IEnumerable<PlanFeatureResponse>>.Ok(features.Select(Map).ToList());
        }
        catch (Exception ex)
        {
            await planFeatureRepository.RollbackTransactionAsync(cancellationToken);
            logger.LogError(ex, "UpdatePlanFeaturesUseCase | Erro ao atualizar features.");
            return Result<IEnumerable<PlanFeatureResponse>>.Failure("Erro inesperado ao atualizar features do plano.");
        }
    }

    private static PlanFeatureResponse Map(PlanFeature feature)
        => new()
        {
            Id = feature.Id,
            SubscriptionPlanId = feature.SubscriptionPlanId,
            FeatureCode = feature.FeatureCode,
            LimitValue = feature.LimitValue,
            Enabled = feature.Enabled
        };
}
