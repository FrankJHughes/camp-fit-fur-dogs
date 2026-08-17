using Microsoft.EntityFrameworkCore;

namespace Frank.Core.EntityFrameworkCore.Tests.Fakes;

public sealed class FakeDbContext : DbContext
{
    public FakeDbContext(DbContextOptions<FakeDbContext> options)
        : base(options) { }
}
