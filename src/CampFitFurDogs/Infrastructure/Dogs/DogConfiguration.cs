using CampFitFurDogs.Domain.Dogs;
using Frank.Core.EntityFrameworkCore.Configurations;
using Frank.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampFitFurDogs.Infrastructure.Dogs;

/// <summary>
/// Configures the EF Core mapping for the <see cref="Dog"/> aggregate root.
/// <para>
/// This configuration defines how the <see cref="Dog"/> domain model is
/// persisted to the database, including conversions for strongly‑typed
/// identifiers, owned value objects, and primitive properties.
/// </para>
/// <para>
/// The configuration follows the conventions of <see cref="AggregateRootConfiguration{TAggregate,TAggregateId}"/>,
/// ensuring consistent mapping across all aggregates in the system.
/// </para>
/// </summary>
public sealed class DogConfiguration : AggregateRootConfiguration<Domain.Dogs.Dog, DogId>
{
    /// <summary>
    /// Gets the database table name used to store <see cref="Dog"/> entities.
    /// </summary>
    protected override string TableName => "dogs";

    /// <summary>
    /// Configures the EF Core entity mapping for the <see cref="Dog"/> aggregate.
    /// <para>
    /// This includes:
    /// <list type="bullet">
    /// <item><description>Identifier conversions for <see cref="DogId"/> and <see cref="UserId"/>.</description></item>
    /// <item><description>Owned value object mappings for <see cref="DogName"/> and <see cref="Breed"/>.</description></item>
    /// <item><description>Primitive property mappings for date of birth and sex.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// All mappings ensure that domain types remain pure and strongly‑typed
    /// while EF Core persists them using primitive values.
    /// </para>
    /// </summary>
    /// <param name="builder">
    /// The <see cref="EntityTypeBuilder{TEntity}"/> used to configure the aggregate.
    /// </param>
    protected override void ConfigureAggregateRoot(EntityTypeBuilder<Domain.Dogs.Dog> builder)
    {
        // DogId → Guid
        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => DogId.From(value))
            .HasColumnName("id");

        // OwnerId → Guid
        builder.Property(d => d.OwnerId)
            .HasConversion(
                id => id.Value,
                value => UserId.From(value))
            .HasColumnName("owner_id")
            .IsRequired();

        // Optional FK relationship (commented out intentionally)
        // builder.HasOne<User>()
        //     .WithMany()
        //     .HasForeignKey(d => d.OwnerId)
        //     .OnDelete(DeleteBehavior.Cascade);

        // DogName (owned value object)
        builder.OwnsOne(d => d.Name, name =>
        {
            name.Property(n => n.Value)
                .HasColumnName("name")
                .IsRequired();
        });

        // Breed (owned value object)
        builder.OwnsOne(d => d.Breed, breed =>
        {
            breed.Property(b => b.Value)
                .HasColumnName("breed")
                .IsRequired();
        });

        // Date of birth
        builder.Property(d => d.DateOfBirth)
            .HasColumnName("date_of_birth")
            .IsRequired();

        // Sex (enum → string)
        builder.Property(d => d.Sex)
            .HasConversion<string>()
            .HasColumnName("sex")
            .IsRequired();
    }
}
