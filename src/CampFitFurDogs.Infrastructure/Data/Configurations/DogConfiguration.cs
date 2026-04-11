using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Infrastructure.Data.Configurations;

public sealed class DogConfiguration : AggregateRootConfiguration<Dog, DogId>
{
    protected override string TableName => "dogs";

    protected override void ConfigureAggregateRoot(EntityTypeBuilder<Dog> builder)
    {
        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => DogId.From(value))
            .HasColumnName("id");

        builder.Property(d => d.OwnerId)
            .HasConversion(
                id => id.Value,
                value => CustomerId.From(value))
            .HasColumnName("owner_id")
            .IsRequired();

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(d => d.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(d => d.Name, name =>
        {
            name.Property(n => n.Value)
                .HasColumnName("name")
                .IsRequired();
        });

        builder.OwnsOne(d => d.Breed, breed =>
        {
            breed.Property(b => b.Value)
                .HasColumnName("breed")
                .IsRequired();
        });

        builder.Property(d => d.DateOfBirth)
            .HasColumnName("date_of_birth")
            .IsRequired();

        builder.Property(d => d.Sex)
            .HasConversion<string>()
            .HasColumnName("sex")
            .IsRequired();
    }
}
