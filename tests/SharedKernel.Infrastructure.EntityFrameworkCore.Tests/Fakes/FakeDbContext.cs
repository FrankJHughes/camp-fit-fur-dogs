using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Infrastructure.EntityFrameworkCore.Tests.Fakes;

public sealed class FakeDbContext : DbContext
{
    public FakeDbContext(DbContextOptions<FakeDbContext> options)
        : base(options) { }
}
