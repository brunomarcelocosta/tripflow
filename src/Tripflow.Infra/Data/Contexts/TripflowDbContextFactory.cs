using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Tripflow.Infra.Data.Contexts;

public sealed class TripflowDbContextFactory : IDesignTimeDbContextFactory<TripflowDbContext>
{
    public TripflowDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Tripflow.API");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<TripflowDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new TripflowDbContext(optionsBuilder.Options, tenantContext: null);
    }
}
