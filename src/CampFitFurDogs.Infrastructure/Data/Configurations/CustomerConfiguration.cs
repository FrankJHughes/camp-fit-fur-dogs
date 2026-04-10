using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Infrastructure.Data.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(c => c.Id);

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

        // PostgreSQL requires explicit owned-property indexing
        builder.HasIndex("Email")
            .IsUnique();
    }
}
