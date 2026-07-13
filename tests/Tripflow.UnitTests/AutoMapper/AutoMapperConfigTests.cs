using Tripflow.Application.AutoMapper;

namespace Tripflow.UnitTests.AutoMapper;

public class AutoMapperConfigTests
{
    [Fact]
    public void Constructor_Should_Create_Valid_Mapper()
    {
        var config = new AutoMapperConfig();

        Assert.NotNull(config.Mapper);
    }
}
