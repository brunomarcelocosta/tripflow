using Microsoft.Extensions.Options;
using Tripflow.Domain.Entities.Identity;
using Tripflow.Domain.Enums;
using Tripflow.Domain.Interfaces;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Integrations.Keycloak;

namespace Tripflow.API.Middlewares;

public sealed class UserProvisioningMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IUserContext userContext,
        IUserProfileRepository userProfileRepository,
        IOptions<KeycloakOptions> keycloakOptions)
    {
        if (!userContext.IsAuthenticated)
        {
            await next(context);
            return;
        }

        if (string.IsNullOrWhiteSpace(userContext.IdentityProviderUserId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                error = "Token inválido: usuário não identificado.",
                data = (object?)null
            });

            return;
        }

        var email = userContext.Email?.Trim();

        var masterEmails = keycloakOptions.Value.MasterAdminEmails ?? [];

        var isMasterAdmin =
            !string.IsNullOrWhiteSpace(email) &&
            masterEmails.Contains(email, StringComparer.OrdinalIgnoreCase);

        var user = await userProfileRepository.GetByIdentityProviderUserIdAsync(
            userContext.IdentityProviderUserId,
            context.RequestAborted);

        if (user is null)
        {
            if (!isMasterAdmin)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;

                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    error = "Usuário ainda não provisionado na TripFlow. Solicite acesso a um administrador.",
                    data = (object?)null
                });

                return;
            }

            var platformTenantId = keycloakOptions.Value.PlatformTenantId;

            if (!platformTenantId.HasValue || platformTenantId.Value == Guid.Empty)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    error = "PlatformTenantId não configurado para provisionamento do master admin.",
                    data = (object?)null
                });

                return;
            }

            user = new UserProfile(
                tenantId: platformTenantId.Value,
                identityProviderUserId: userContext.IdentityProviderUserId,
                fullName: userContext.Name ?? email ?? "Master Admin",
                email: email ?? "unknown@email.com",
                phone: null,
                status: UserStatus.Invited,
                createdBy: "system");

            user.Activate("system");

            await userProfileRepository.AddAsync(user, context.RequestAborted);
            await userProfileRepository.SaveChangesAsync(context.RequestAborted);

            await next(context);
            return;
        }

        if (isMasterAdmin && user.Status == UserStatus.Invited)
        {
            var trackedUser = await userProfileRepository.GetTrackedByIdentityProviderUserIdAsync(
                userContext.IdentityProviderUserId,
                context.RequestAborted);

            if (trackedUser is not null)
            {
                trackedUser.Activate("system");
                await userProfileRepository.UpdateAsync(trackedUser, context.RequestAborted);
            }
        }

        if (user.Status != UserStatus.Active)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            var message = user.Status switch
            {
                UserStatus.Invited => "Acesso pendente de ativação. Verifique o convite ou fale com o suporte.",
                UserStatus.Blocked => "Usuário bloqueado. Entre em contato com um administrador.",
                UserStatus.Removed => "Usuário removido da plataforma.",
                _ => "Usuário sem permissão para acessar a plataforma."
            };

            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                error = message,
                data = (object?)null
            });

            return;
        }

        await next(context);
    }
}

