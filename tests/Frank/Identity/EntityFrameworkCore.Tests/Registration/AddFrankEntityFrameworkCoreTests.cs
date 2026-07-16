using Frank.Core.Application.Abstractions.UnitOfWork;
using Frank.Identity.EntityFrameworkCore.Persistence;
using Frank.Identity.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.EntityFrameworkCore.Tests.Registration;

public sealed class AddFrankEntityFrameworkCoreTests
{
    [Fact]
    public void AddFrankEfCore_registers_IFrankIdentityUnitOfWork()
    {
        var services = new ServiceCollection();

        services.AddDbContext<FrankIdentityDbContext>(o => o.UseInMemoryDatabase("test"));
        services.AddFrankIdentityUnitOfWork();

        var provider = services.BuildServiceProvider();

        var uow = provider.GetRequiredService<IFrankIdentityUnitOfWork>();

        uow.Should().BeOfType<FrankIdentityUnitOfWork>();
    }
}
