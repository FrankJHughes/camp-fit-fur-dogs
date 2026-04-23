using SharedKernel.DependencyInjection;
namespace SharedKernel.Tests.DependencyInjection;

public sealed class AutoRegistration_MultiAssemblyTests
{
    [Fact]
    public void AddApplication_scans_multiple_assemblies()
    {
        var services = new ServiceCollection();

        services.AddSharedKernel(
            new[] { typeof(SharedKernel.Tests.DependencyInjection.Fakes.AssemblyMarker).Assembly }
        );

        using var provider = services.BuildServiceProvider();

        provider.GetService<ICommandHandler<SharedKernel.Tests.DependencyInjection.Fakes.FakeCommand, SharedKernel.Tests.DependencyInjection.Fakes.FakeResponse>>()
            .Should().NotBeNull();

        provider.GetService<IQueryHandler<SharedKernel.Tests.DependencyInjection.Fakes.FakeQuery, SharedKernel.Tests.DependencyInjection.Fakes.FakeResponse>>()
            .Should().NotBeNull();
    }
}
