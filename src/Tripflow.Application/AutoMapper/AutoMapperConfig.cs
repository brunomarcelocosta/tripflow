using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;

namespace Tripflow.Application.AutoMapper;

public class AutoMapperConfig
{
    private IMapper _mapper;
    public IMapper Mapper => _mapper;

    public AutoMapperConfig()
    {
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new DomainToResponse());
        }, NullLoggerFactory.Instance);

        _mapper = mapperConfig.CreateMapper();
    }

}
