using Microsoft.EntityFrameworkCore;

using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Infrastructure.Data.Configurations;

namespace CampFitFurDogs.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.ApplyConfiguration(new CustomerConfiguration());
    }
}
