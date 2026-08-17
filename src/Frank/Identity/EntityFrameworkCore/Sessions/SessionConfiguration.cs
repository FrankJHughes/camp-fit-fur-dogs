using Frank.Core.EntityFrameworkCore.Configurations;
using Frank.Identity.Domain.Sessions;
using Frank.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Frank.Identity.EntityFrameworkCore.Sessions;

/// <summary>
/// Configures the EF Core persistence mapping for the <see cref="Session"/> aggregate.
/// <para>
/// This configuration maps all value objects to their underlying primitive types,
/// defines column names, and applies constraints such as uniqueness and required fields.
/// </para>
/// <para>
/// The mapping ensures that the <see cref="Session"/> domain model remains pure
/// while EF Core handles serialization and materialization.
/// </para>
/// </summary>
public sealed class SessionConfiguration : AggregateRootConfiguration<Session, SessionId>
{
    /// <summary>
    /// Gets the database table name used for storing <see cref="Session"/> entities.
    /// </summary>
    protected override string TableName => "sessions";

    /// <summary>
    /// Configures the EF Core entity mapping for the <see cref="Session"/> aggregate.
    /// <para>
    /// All value objects are converted to and from their underlying primitive types
    /// using <see cref="ValueConverter"/>s. Column names follow snake_case conventions
    /// to align with PostgreSQL standards.
    /// </para>
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the mapping.</param>
    protected override void ConfigureAggregateRoot(EntityTypeBuilder<Session> builder)
    {
        //
        // ID (SessionId VO)
        //
        /// <summary>
        /// Maps the <see cref="SessionId"/> value object to its underlying <see cref="Guid"/>
        /// and configures the database column name.
        /// </summary>
        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => SessionId.From(value))
            .HasColumnName("id");

        //
        // TokenHash (SessionTokenHash VO)
        //
        /// <summary>
        /// Maps the <see cref="SessionTokenHash"/> value object to its underlying string,
        /// configures the column name, and enforces uniqueness.
        /// </summary>
        builder.Property(s => s.TokenHash)
            .HasConversion(
                v => v.Value,
                v => SessionTokenHash.From(v))
            .HasColumnName("token_hash")
            .IsRequired();

        builder.HasIndex(s => s.TokenHash)
            .IsUnique();

        //
        // OwnerId (UserId VO)
        //
        /// <summary>
        /// Maps the <see cref="UserId"/> value object to its underlying <see cref="Guid"/>
        /// and configures the column name.
        /// </summary>
        builder.Property(s => s.OwnerId)
            .HasConversion(
                v => v.Value,
                v => UserId.From(v))
            .HasColumnName("owner_id")
            .IsRequired();

        //
        // CreatedAt (timestamp)
        //
        /// <summary>
        /// Maps the <c>CreatedAt</c> timestamp to the <c>created_at</c> column.
        /// This value is always required because sessions must have a creation time.
        /// </summary>
        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        //
        // RevokedAt (nullable timestamp)
        //
        /// <summary>
        /// Maps the <c>RevokedAt</c> timestamp to the <c>revoked_at</c> column.
        /// This value is nullable because sessions may never be revoked.
        /// </summary>
        builder.Property(s => s.RevokedAt)
            .HasColumnName("revoked_at")
            .IsRequired(false);
    }
}
