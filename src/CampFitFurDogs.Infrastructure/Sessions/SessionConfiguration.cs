using CampFitFurDogs.Domain.Sessions;
using CampFitFurDogs.Domain.Customers;
using Frank.Infrastructure.EntityFrameworkCore.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampFitFurDogs.Infrastructure.Sessions;

public sealed class SessionConfiguration : AggregateRootConfiguration<Domain.Sessions.Session, SessionId>
{
    protected override string TableName => "sessions";

    protected override void ConfigureAggregateRoot(EntityTypeBuilder<Domain.Sessions.Session> builder)
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
        // OwnerId (CustomerId VO)
        //
        builder.Property(s => s.OwnerId)
            .HasConversion(
                v => v.Value,
                v => CustomerId.From(v))
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
