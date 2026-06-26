using Frank.Abstractions.Command;
using Frank.Abstractions.Query;

namespace Frank.Tests.DependencyInjection;

public sealed class AutoRegistration_MultiAssemblyTests
{
    [Fact]
    public void AddApplication_scans_multiple_assemblies()
    {
        var services = new ServiceCollection();

        services.AddFrank(
            new[] { typeof(Frank.Tests.DependencyInjection.Fakes.AssemblyMarker).Assembly }
        );

        using var provider = services.BuildServiceProvider();

        provider.GetService<ICommandHandler<Frank.Tests.DependencyInjection.Fakes.FakeCommand, Frank.Tests.DependencyInjection.Fakes.FakeResponse>>()
            .Should().NotBeNull();

        provider.GetService<IQueryHandler<Frank.Tests.DependencyInjection.Fakes.FakeQuery, Frank.Tests.DependencyInjection.Fakes.FakeResponse>>()
            .Should().NotBeNull();
    }
}
