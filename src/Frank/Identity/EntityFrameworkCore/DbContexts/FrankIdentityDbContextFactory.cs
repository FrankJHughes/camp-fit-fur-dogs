using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Frank.Identity.EntityFrameworkCore.DbContexts;

public sealed class FrankIdentityDesignTimeDbContextFactory : IDesignTimeDbContextFactory<FrankIdentityDbContext>
{
    public FrankIdentityDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables() // REQUIRED for GitHub Actions
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        var options = new DbContextOptionsBuilder<FrankIdentityDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new FrankIdentityDbContext(options);
    }
}
