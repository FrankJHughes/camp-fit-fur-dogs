using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder model)
    {
        System.Diagnostics.Debug.WriteLine("EF MODEL LOADED FROM: " + GetType().Assembly.Location);

        model.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
