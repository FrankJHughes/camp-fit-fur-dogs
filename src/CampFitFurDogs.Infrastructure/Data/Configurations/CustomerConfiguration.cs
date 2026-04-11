using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Infrastructure.Data.Configurations;

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

        builder.Property(c => c.FirstName)
            .HasColumnName("first_name")
            .IsRequired();

        builder.Property(c => c.LastName)
            .HasColumnName("last_name")
            .IsRequired();

        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("email")
                .IsRequired();

            // PostgreSQL requires explicit owned-property indexing
            email.HasIndex(e => e.Value)
                 .IsUnique();
        });

        builder.OwnsOne(c => c.Phone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("phone")
                .IsRequired();
        });

        builder.OwnsOne(c => c.PasswordHash, pw =>
        {
            pw.Property(p => p.Value)
                .HasColumnName("password_hash")
                .IsRequired();
        });
    }
}
