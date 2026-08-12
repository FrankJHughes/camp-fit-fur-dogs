using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Persistence;

/// <summary>
/// Represents the Entity Framework Core database context for the CampFitFurDogs
/// application.
/// <para>
/// This context is responsible for materializing and persisting aggregates,
/// applying entity configurations, and serving as the unit of work boundary
/// for infrastructure‑layer operations.
/// </para>
/// <para>
/// All entity mappings are applied automatically via
/// <c>ApplyConfigurationsFromAssembly</c>, ensuring that EF Core configuration
/// classes located in the same assembly are discovered without manual
/// registration.
/// </para>
/// </summary>
public sealed class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class using
    /// the provided <see cref="DbContextOptions{TContext}"/>.
    /// </summary>
    /// <param name="options">
    /// The configuration options used to construct the database context.
    /// </param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// Applies model configurations during EF Core model creation.
    /// <para>
    /// This method:
    /// <list type="bullet">
    /// <item><description>Logs the assembly location for debugging purposes.</description></item>
    /// <item><description>Automatically applies all <c>IEntityTypeConfiguration</c> implementations found in the assembly.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="model">The model builder used to configure entity mappings.</param>
    protected override void OnModelCreating(ModelBuilder model)
    {
        System.Diagnostics.Debug.WriteLine("EF MODEL LOADED FROM: " + GetType().Assembly.Location);

        model.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
