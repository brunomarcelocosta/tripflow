using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Tripflow.Application.AutoMapper;

namespace Tripflow.UnitTests.Utils;

public static class AutoMapperTestHelper
{
    public static IMapper CreateMapper()
    {
        var loggerFactory = NullLoggerFactory.Instance;
        var config = new MapperConfiguration(cfg => cfg.AddProfile(new DomainToResponse()), loggerFactory);
        config.AssertConfigurationIsValid();
        return config.CreateMapper();
    }
}
