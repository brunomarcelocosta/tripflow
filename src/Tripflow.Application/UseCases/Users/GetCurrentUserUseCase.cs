using AutoMapper;
using Tripflow.Application.DTOs.Common;
using Tripflow.Application.DTOs.Responses.Subscriptions;
using Tripflow.Application.DTOs.Responses.Users;
using Tripflow.Application.UseCases.Users.Interfaces;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Application.UseCases.Platform;

namespace Tripflow.Application.UseCases.Users;

public sealed class GetCurrentUserUseCase(
    IUserContext userContext,
    ITenantContext tenantContext,
    IUserProfileRepository userProfileRepository,
    ITenantSubscriptionRepository tenantSubscriptionRepository,
    IMapper mapper) : IGetCurrentUserUseCase
{
    public async Task<Result<GetCurrentUserResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!userContext.IsAuthenticated)
            return Result<GetCurrentUserResponse>.Forbidden();

        if (string.IsNullOrWhiteSpace(userContext.IdentityProviderUserId))
            return Result<GetCurrentUserResponse>.Failure("Usuário autenticado sem identificador válido.");

        var profile = await userProfileRepository.GetByIdentityProviderUserIdAsync(
            userContext.IdentityProviderUserId,
            cancellationToken);

        if (profile is null)
            return Result<GetCurrentUserResponse>.Failure("Perfil de usuário não encontrado.");

        var response = mapper.Map<GetCurrentUserResponse>(profile);

        TenantEntitlementsResponse? entitlements = null;

        if (tenantContext.HasTenant && tenantContext.TenantId is not null)
        {
            var subscription = await tenantSubscriptionRepository.GetByTenantIdWithPlanFeaturesAsync(
                tenantContext.TenantId.Value,
                cancellationToken);
            entitlements = PublicSubscriptionMapping.MapEntitlements(subscription);
        }

        response = response with { Entitlements = entitlements };

        return Result<GetCurrentUserResponse>.Ok(response);
    }
}
