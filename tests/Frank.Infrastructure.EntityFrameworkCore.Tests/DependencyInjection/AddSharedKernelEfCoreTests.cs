using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Frank.Abstractions;
using Frank.Infrastructure.EntityFrameworkCore;
using Frank.Infrastructure.EntityFrameworkCore.Tests.Fakes;
using Xunit;

namespace Frank.Infrastructure.EntityFrameworkCore.Tests.DependencyInjection;

public sealed class AddFrankEfCoreTests
{
    [Fact]
    public void AddFrankEfCore_registers_IUnitOfWork()
    {
        var services = new ServiceCollection();

        services.AddDbContext<FakeDbContext>(o => o.UseInMemoryDatabase("test"));
        services.AddFrankEfCore<FakeDbContext>(
            [typeof(FakeDbContext).Assembly]
        );

        var provider = services.BuildServiceProvider();

        var uow = provider.GetRequiredService<IUnitOfWork>();

        uow.Should().BeOfType<EfUnitOfWork<FakeDbContext>>();
    }
}
