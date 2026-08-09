using Frank.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Frank.Core.EntityFrameworkCore.Configurations;

/// <summary>
/// Provides a base Entity Framework Core configuration for aggregate roots,
/// ensuring consistent mapping of identifiers, domain event suppression, and
/// table naming conventions across all aggregates.
///
/// <para>
/// This configuration enforces the following rules:
/// <list type="bullet">
///   <item><description>
///   Aggregate identifiers (<see cref="AggregateRoot{TId}.Id"/>) are mapped as
///   explicit keys and are never database‑generated.
///   </description></item>
///   <item><description>
///   Domain events (<see cref="AggregateRoot{TId}.DomainEvents"/>) are ignored
///   and never persisted.
///   </description></item>
///   <item><description>
///   Derived configurations must specify the table name and may add additional
///   property mappings, relationships, indexes, and constraints.
///   </description></item>
/// </list>
/// </para>
///
/// <para>
/// This base class centralizes common EF Core mapping rules for aggregate roots,
/// ensuring consistency across vertical slices and reducing duplication.
/// </para>
/// </summary>
/// <typeparam name="TAggregateRoot">
/// The aggregate root type being configured.
/// </typeparam>
/// <typeparam name="TId">
/// The identifier type used by the aggregate root.
/// </typeparam>
public abstract class AggregateRootConfiguration<TAggregateRoot, TId>
    : IEntityTypeConfiguration<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TId>
    where TId : AggregateId
{
    /// <summary>
    /// Configures the aggregate root entity, applying standard conventions and
    /// delegating additional configuration to derived classes.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="EntityTypeBuilder{TAggregateRoot}"/> used to configure the entity.
    /// </param>
    public void Configure(EntityTypeBuilder<TAggregateRoot> builder)
    {
        builder.ToTable(TableName);

        // Explicit key mapping (works even if Id has a private setter)
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        // Domain events are never persisted
        builder.Ignore(a => a.DomainEvents);

        // Allow derived configurations to add properties, relationships, indexes, etc.
        ConfigureAggregateRoot(builder);
    }

    /// <summary>
    /// Gets the table name used for the aggregate root.
    /// Derived configurations must provide a concrete value.
    /// </summary>
    protected abstract string TableName { get; }

    /// <summary>
    /// Allows derived configurations to define additional EF Core mappings such
    /// as properties, relationships, indexes, and constraints.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="EntityTypeBuilder{TAggregateRoot}"/> used to configure the entity.
    /// </param>
    protected abstract void ConfigureAggregateRoot(EntityTypeBuilder<TAggregateRoot> builder);
}
