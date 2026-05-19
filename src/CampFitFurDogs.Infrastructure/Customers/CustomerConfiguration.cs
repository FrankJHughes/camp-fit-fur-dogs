using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SharedKernel.Infrastructure.EntityFrameworkCore.Configurations;

using CampFitFurDogs.Domain.Customers;

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

        // FirstName VO
        builder.OwnsOne(c => c.FirstName, fn =>
        {
            fn.Property(f => f.Value)
              .HasColumnName("first_name")
              .IsRequired();
        });

        // LastName VO
        builder.OwnsOne(c => c.LastName, ln =>
        {
            ln.Property(l => l.Value)
              .HasColumnName("last_name")
              .IsRequired();
        });

        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("email")
                .IsRequired();

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
