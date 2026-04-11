using Microsoft.EntityFrameworkCore;

using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Data.Configurations;

namespace CampFitFurDogs.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Dog> Dogs => Set<Dog>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.ApplyConfiguration(new CustomerConfiguration());
        model.ApplyConfiguration(new DogConfiguration());
    }
}
