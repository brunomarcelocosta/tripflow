using Microsoft.Extensions.DependencyInjection;
using Tripflow.Application.AutoMapper;

namespace Tripflow.Infra.IoC.DI;

public static class AutoMapperDI
{
    public static void AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(DomainToResponse));
    }
}