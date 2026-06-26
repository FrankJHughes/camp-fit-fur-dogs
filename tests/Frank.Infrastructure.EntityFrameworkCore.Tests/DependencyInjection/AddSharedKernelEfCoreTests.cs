using FluentAssertions;
using Frank.Abstractions.UnitOfWork;
using Frank.Infrastructure.EntityFrameworkCore.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Frank.Infrastructure.EntityFrameworkCore.Tests.DependencyInjection;

public sealed class AddFrankEfCoreTests
{
    [Fact]
    public void AddFrankEfCore_registers_IUnitOfWork()
    {
        var services = new ServiceCollection();

        services.AddDbContext<FakeDbContext>(o => o.UseInMemoryDatabase("test"));
        services.AddFrankEntityFrameworkCoreInfrastructure<FakeDbContext>();

        var provider = services.BuildServiceProvider();

        var uow = provider.GetRequiredService<IUnitOfWork>();

        uow.Should().BeOfType<EntityFrameworkCoreUnitOfWork<FakeDbContext>>();
    }
}
