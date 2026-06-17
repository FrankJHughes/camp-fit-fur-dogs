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
        // OPTIONAL VALUE OBJECTS
        //
        builder.Property(c => c.Phone)
            .HasConversion(
                v => v == null ? null : v.Value,
                v => v == null ? null : PhoneNumber.From(v))
            .HasColumnName("phone")
            .IsRequired(false);

        //
        // REQUIRED ExternalId (post–US‑184)
        //
        builder.OwnsOne(c => c.ExternalId, ext =>
        {
            // ExternalId is a single-property VO containing only "Value"
            ext.Property(e => e.Value)
                .HasColumnName("external_id")
                .HasMaxLength(200)
                .IsRequired();

            // Unique constraint on the external identity
            ext.HasIndex(e => e.Value)
                .IsUnique();
        });
    }
}
