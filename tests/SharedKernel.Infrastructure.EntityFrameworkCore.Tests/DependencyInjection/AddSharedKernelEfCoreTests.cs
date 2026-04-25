using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstractions;
using SharedKernel.Infrastructure.EntityFrameworkCore;
using SharedKernel.Infrastructure.EntityFrameworkCore.Tests.Fakes;
using Xunit;

namespace SharedKernel.Infrastructure.EntityFrameworkCore.Tests.DependencyInjection;

public sealed class AddSharedKernelEfCoreTests
{
    [Fact]
    public void AddSharedKernelEfCore_registers_IUnitOfWork()
    {
        var services = new ServiceCollection();

        services.AddDbContext<FakeDbContext>(o => o.UseInMemoryDatabase("test"));
        services.AddSharedKernelEfCore<FakeDbContext>();

        var provider = services.BuildServiceProvider();

        var uow = provider.GetRequiredService<IUnitOfWork>();

        uow.Should().BeOfType<EfUnitOfWork<FakeDbContext>>();
    }
}
