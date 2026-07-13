using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Tripflow.Domain.Interfaces.Security;
using Tripflow.Infra.Security;

namespace Tripflow.Infra.IoC.DI;

public static class SecurityDI
{
    public static IServiceCollection AddTripflowSecurity(this IServiceCollection services)
    {
        services.AddDataProtection();
        services.AddScoped<ISecretProtector, DataProtectionSecretProtector>();
        return services;
    }
}
