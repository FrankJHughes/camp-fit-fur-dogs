using Microsoft.EntityFrameworkCore;

namespace Frank.Identity.EntityFrameworkCore.DbContexts;

/// <summary>
/// Entity Framework Core database context for the Identity subsystem.
/// <para>
/// This context is responsible for materializing Identity domain aggregates,
/// value objects, and persistence configurations. It loads all EF Core
/// configuration classes from the assembly containing
/// <see cref="FrankIdentityDbContext"/>.
/// </para>
/// <para>
/// The context is intentionally thin: all mapping logic is delegated to
/// configuration classes via <see cref="ModelBuilder.ApplyConfigurationsFromAssembly"/>.
/// </para>
/// </summary>
public sealed class FrankIdentityDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FrankIdentityDbContext"/>
    /// using the provided <see cref="DbContextOptions{TContext}"/>.
    /// </summary>
    /// <param name="options">
    /// The configuration options used to construct the context, including
    /// connection settings, provider selection, and EF Core behaviors.
    /// </param>
    public FrankIdentityDbContext(DbContextOptions<FrankIdentityDbContext> options)
        : base(options) { }

    /// <summary>
    /// Configures the EF Core model by applying all entity type configuration
    /// classes found in the current assembly.
    /// <para>
    /// This ensures that all Identity‑related entity mappings, conversions,
    /// owned types, and constraints are automatically discovered and applied.
    /// </para>
    /// </summary>
    /// <param name="model">The model builder used to configure entity mappings.</param>
    protected override void OnModelCreating(ModelBuilder model)
    {
        System.Diagnostics.Debug.WriteLine("EF MODEL LOADED FROM: " + GetType().Assembly.Location);

        model.ApplyConfigurationsFromAssembly(typeof(FrankIdentityDbContext).Assembly);
    }
}
