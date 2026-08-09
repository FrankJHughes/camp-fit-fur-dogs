using Frank.Core.EntityFrameworkCore.Configurations;
using Frank.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Frank.Identity.EntityFrameworkCore.Users;

/// <summary>
/// Configures the EF Core persistence mapping for the <see cref="User"/> aggregate.
/// <para>
/// This configuration maps all value objects to their underlying primitive types,
/// defines column names, and applies required/optional constraints and uniqueness
/// rules. Complex value objects are mapped using <see cref="OwnedNavigationBuilder"/>
/// to preserve domain purity while enabling relational persistence.
/// </para>
/// <para>
/// The mapping ensures that the <see cref="User"/> domain model remains immutable
/// and expressive, while EF Core handles serialization and materialization.
/// </para>
/// </summary>
public sealed class UserConfiguration : AggregateRootConfiguration<User, UserId>
{
    /// <summary>
    /// Gets the database table name used for storing <see cref="User"/> entities.
    /// </summary>
    protected override string TableName => "users";

    /// <summary>
    /// Configures the EF Core entity mapping for the <see cref="User"/> aggregate.
    /// <para>
    /// All value objects are converted to and from their underlying primitive types
    /// or mapped as owned types. Column names follow snake_case conventions to align
    /// with PostgreSQL standards.
    /// </para>
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the mapping.</param>
    protected override void ConfigureAggregateRoot(EntityTypeBuilder<User> builder)
    {
        /// <summary>
        /// Maps the <see cref="UserId"/> value object to its underlying <see cref="Guid"/>
        /// and configures the database column name.
        /// </summary>
        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => UserId.From(value))
            .HasColumnName("id");

        // FirstName VO (required)
        /// <summary>
        /// Maps the <see cref="FirstName"/> value object as an owned type.
        /// The underlying <c>Value</c> property is stored in the <c>first_name</c> column.
        /// </summary>
        builder.OwnsOne(c => c.FirstName, fn =>
        {
            fn.Property(f => f.Value)
              .HasColumnName("first_name")
              .IsRequired();
        });

        // LastName VO (required)
        /// <summary>
        /// Maps the <see cref="LastName"/> value object as an owned type.
        /// The underlying <c>Value</c> property is stored in the <c>last_name</c> column.
        /// </summary>
        builder.OwnsOne(c => c.LastName, ln =>
        {
            ln.Property(l => l.Value)
              .HasColumnName("last_name")
              .IsRequired();
        });

        // Email VO (required)
        /// <summary>
        /// Maps the <see cref="Email"/> value object as an owned type.
        /// The underlying <c>Value</c> property is stored in the <c>email</c> column.
        /// </summary>
        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("email")
                .IsRequired();
        });

        //
        // OPTIONAL VALUE OBJECTS
        //
        /// <summary>
        /// Maps the optional <see cref="PhoneNumber"/> value object using a value converter.
        /// The column is nullable because phone numbers are not required for user accounts.
        /// </summary>
        builder.Property(c => c.Phone)
            .HasConversion(
                v => v == null ? null : v.Value,
                v => v == null ? null : PhoneNumber.From(v))
            .HasColumnName("phone")
            .IsRequired(false);

        //
        // REQUIRED ExternalId (post–US‑184)
        //
        /// <summary>
        /// Maps the <see cref="ExternalId"/> value object as an owned type.
        /// The underlying <c>Value</c> property is stored in the <c>external_id</c> column.
        /// A uniqueness constraint is applied to ensure each external identity maps to
        /// exactly one internal user.
        /// </summary>
        builder.OwnsOne(c => c.ExternalId, ext =>
        {
            ext.Property(e => e.Value)
                .HasColumnName("external_id")
                .HasMaxLength(200)
                .IsRequired();

            ext.HasIndex(e => e.Value)
                .IsUnique();
        });
    }
}
