using CampFitFurDogs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Use Npgsql provider so the model snapshot matches runtime
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=does_not_matter;Username=does_not_matter;Password=does_not_matter");

        return new AppDbContext(optionsBuilder.Options);
    }
}
