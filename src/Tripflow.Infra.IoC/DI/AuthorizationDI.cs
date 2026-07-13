using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Tripflow.Domain.Interfaces;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Services;

namespace Tripflow.Infra.IoC.DI;

public static class AuthorizationDI
{
    public static IServiceCollection AddTripflowAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IUserPermissionService, UserPermissionService>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<ITenantRoleProvisioningService, TenantRoleProvisioningService>();

        return services;
    }
}
