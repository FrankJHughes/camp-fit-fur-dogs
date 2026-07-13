using Microsoft.EntityFrameworkCore;

namespace Frank.Identity.EntityFrameworkCore.Persistence;

public sealed class FrankIdentityDbContext : DbContext
{
    public FrankIdentityDbContext(DbContextOptions<FrankIdentityDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder model)
    {
        System.Diagnostics.Debug.WriteLine("EF MODEL LOADED FROM: " + GetType().Assembly.Location);

        model.ApplyConfigurationsFromAssembly(typeof(FrankIdentityDbContext).Assembly);
    }
}
