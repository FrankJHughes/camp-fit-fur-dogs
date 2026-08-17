using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Frank.Identity.EntityFrameworkCore.DbContexts;

/// <summary>
/// Provides a design‑time factory for creating <see cref="FrankIdentityDbContext"/>
/// instances when invoked by Entity Framework Core tooling.
/// <para>
/// EF Core requires a design‑time factory when the application's normal startup
/// path does not provide a fully configured <see cref="DbContext"/>—for example,
/// during migrations, scaffolding, or CLI operations.
/// </para>
/// <para>
/// This factory loads configuration from:
/// </para>
/// <list type="bullet">
/// <item><description><c>appsettings.json</c></description></item>
/// <item><description><c>appsettings.Development.json</c></description></item>
/// <item><description>Environment variables (required for CI/CD pipelines)</description></item>
/// </list>
/// <para>
/// It then constructs a <see cref="DbContextOptions{TContext}"/> using the
/// configured PostgreSQL connection string.
/// </para>
/// </summary>
public sealed class FrankIdentityDesignTimeDbContextFactory
    : IDesignTimeDbContextFactory<FrankIdentityDbContext>
{
    /// <summary>
    /// Creates a new <see cref="FrankIdentityDbContext"/> instance for use by
    /// EF Core design‑time tooling such as <c>dotnet ef migrations</c>.
    /// </summary>
    /// <param name="args">
    /// Optional command‑line arguments supplied by EF Core tooling.
    /// </param>
    /// <returns>
    /// A fully configured <see cref="FrankIdentityDbContext"/> instance using
    /// the resolved connection string.
    /// </returns>
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
