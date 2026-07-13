using Microsoft.Extensions.DependencyInjection;
using Tripflow.Domain.Interfaces.Contexts;
using Tripflow.Infra.Auth;
using Tripflow.Infra.Context;

namespace Tripflow.Infra.IoC.DI;

public static class ContextDI
{
    public static IServiceCollection AddRequestContexts(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<IUserContext, UserContext>();

        services.AddScoped<TenantContext>();

        services.AddScoped<ITenantContext>(provider =>
            provider.GetRequiredService<TenantContext>());

        services.AddScoped<ITenantContextSetter>(provider =>
            provider.GetRequiredService<TenantContext>());

        services.AddScoped<ISupportContext, SupportContext>();

        return services;
    }
}
