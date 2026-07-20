using Frank.Core.EntityFrameworkCore.Configurations;
using Frank.Identity.Domain.Sessions;
using Frank.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Frank.Identity.EntityFrameworkCore.Sessions;

public sealed class SessionConfiguration : AggregateRootConfiguration<Session, SessionId>
{
    protected override string TableName => "sessions";

    protected override void ConfigureAggregateRoot(EntityTypeBuilder<Session> builder)
    {
        //
        // ID (SessionId VO)
        //
        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => SessionId.From(value))
            .HasColumnName("id");

        //
        // TokenHash (SessionTokenHash VO)
        //
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
        builder.Property(s => s.OwnerId)
            .HasConversion(
                v => v.Value,
                v => UserId.From(v))
            .HasColumnName("owner_id")
            .IsRequired();

        //
        // CreatedAt (timestamp)
        //
        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        //
        // RevokedAt (nullable timestamp)
        //
        builder.Property(s => s.RevokedAt)
            .HasColumnName("revoked_at")
            .IsRequired(false);
    }
}
