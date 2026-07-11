using FluentAssertions;
using Frank.Abstractions.UnitOfWork;
using Frank.Infrastructure.EntityFrameworkCore.Persistence;
using Frank.Infrastructure.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Frank.Infrastructure.EntityFrameworkCore.Tests.DependencyInjection;

public sealed class AddFrankEfCoreTests
{
    [Fact]
    public void AddFrankEfCore_registers_IFrankIdentityUnitOfWork()
    {
        var services = new ServiceCollection();

        services.AddDbContext<FrankIdentityDbContext>(o => o.UseInMemoryDatabase("test"));
        services.AddFrankEntityFrameworkCoreInfrastructure();

        var provider = services.BuildServiceProvider();

        var uow = provider.GetRequiredService<IFrankIdentityUnitOfWork>();

        uow.Should().BeOfType<FrankIdentityUnitOfWork>();
    }
}
