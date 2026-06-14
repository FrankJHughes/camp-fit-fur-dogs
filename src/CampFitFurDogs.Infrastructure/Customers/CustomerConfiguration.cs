using CampFitFurDogs.Domain.Customers;
using Frank.Infrastructure.EntityFrameworkCore.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampFitFurDogs.Infrastructure.Customers;

public sealed class CustomerConfiguration : AggregateRootConfiguration<Customer, CustomerId>
{
    protected override string TableName => "customers";

    protected override void ConfigureAggregateRoot(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => CustomerId.From(value))
            .HasColumnName("id");

        // FirstName VO (required)
        builder.OwnsOne(c => c.FirstName, fn =>
        {
            fn.Property(f => f.Value)
              .HasColumnName("first_name")
              .IsRequired();
        });

        // LastName VO (required)
        builder.OwnsOne(c => c.LastName, ln =>
        {
            ln.Property(l => l.Value)
              .HasColumnName("last_name")
              .IsRequired();
        });

        // Email VO (required)
        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("email")
                .IsRequired();
        });

        //
        // OPTIONAL VALUE OBJECTS — mapped via nullable scalar + converter
        //

        builder.Property(c => c.Phone)
            .HasConversion(
                v => v == null ? null : v.Value,
                v => v == null ? null : PhoneNumber.From(v))
            .HasColumnName("phone")
            .IsRequired(false);

        builder.Property(c => c.PasswordHash)
            .HasConversion(
                v => v == null ? null : v.Value,
                v => v == null ? null : PasswordHash.From(v))
            .HasColumnName("password_hash")
            .IsRequired(false);

        builder.Property(c => c.ExternalAuthProviderId)
            .HasConversion(
                v => v == null ? null : v.Value,
                v => v == null ? null : ExternalAuthProviderId.From(v))
            .HasColumnName("external_auth_provider_id")
            .HasMaxLength(200)
            .IsRequired(false);
    }
}
