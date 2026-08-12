using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.Infrastructure.Persistence;

/// <summary>
/// Provides a design‑time factory for creating instances of <see cref="AppDbContext"/>
/// when executing Entity Framework Core tooling such as migrations.
/// <para>
/// EF Core requires a design‑time context factory when the application's normal
/// runtime bootstrapping (e.g., dependency injection, configuration loading)
/// is not available. This factory ensures that migrations can be generated and
/// applied consistently across local development and CI environments.
/// </para>
/// <para>
/// The factory loads configuration from:
/// <list type="bullet">
/// <item><description><c>appsettings.json</c></description></item>
/// <item><description><c>appsettings.Development.json</c></description></item>
/// <item><description>Environment variables (required for GitHub Actions)</description></item>
/// </list>
/// </para>
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <summary>
    /// Creates a new <see cref="AppDbContext"/> instance for design‑time tooling.
    /// <para>
    /// This method:
    /// <list type="bullet">
    /// <item><description>Builds a configuration root from JSON files and environment variables.</description></item>
    /// <item><description>Retrieves the <c>DefaultConnection</c> connection string.</description></item>
    /// <item><description>Constructs <see cref="DbContextOptions{TContext}"/> using Npgsql.</description></item>
    /// <item><description>Returns a fully configured <see cref="AppDbContext"/>.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="args">Optional command‑line arguments supplied by EF Core tooling.</param>
    /// <returns>A fully configured <see cref="AppDbContext"/> instance.</returns>
    public AppDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables() // REQUIRED for GitHub Actions
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
