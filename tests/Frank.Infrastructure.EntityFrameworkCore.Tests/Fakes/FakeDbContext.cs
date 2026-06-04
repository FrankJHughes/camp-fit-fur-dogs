using Microsoft.EntityFrameworkCore;

namespace Frank.Infrastructure.EntityFrameworkCore.Tests.Fakes;

public sealed class FakeDbContext : DbContext
{
    public FakeDbContext(DbContextOptions<FakeDbContext> options)
        : base(options) { }
}
