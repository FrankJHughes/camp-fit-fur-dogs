using SharedKernel.DependencyInjection;
using SharedKernel.Events;
using SharedKernel.Tests.DependencyInjection.Fakes;

namespace SharedKernel.Tests.DependencyInjection;

public sealed class AutoRegistrationTests
{
    [Fact]
    public void AddApplication_registers_command_handlers()
    {
        var services = new ServiceCollection();

        services.AddSharedKernel(
            [typeof(FakeCommand).Assembly]
        );

        using var provider = services.BuildServiceProvider();

        var handler = provider.GetService<ICommandHandler<FakeCommand, FakeResponse>>();
        handler.Should().NotBeNull();
    }

    [Fact]
    public void AddApplication_registers_query_handlers()
    {
        var services = new ServiceCollection();

        services.AddSharedKernel(
            [typeof(Fakes.AssemblyMarker).Assembly]
        );

        using var provider = services.BuildServiceProvider();

        var handler = provider.GetService<IQueryHandler<FakeQuery, FakeResponse>>();

        handler.Should().NotBeNull();
    }

    [Fact]
    public void AddApplication_registers_validators()
    {
        var services = new ServiceCollection();

        services.AddSharedKernel(
            new[] { typeof(SharedKernel.Tests.DependencyInjection.Fakes.AssemblyMarker).Assembly }
        );

        using var provider = services.BuildServiceProvider();

        var validators = provider.GetServices<IValidator<SharedKernel.Tests.DependencyInjection.Fakes.FakeCommand>>();

        validators.Should().NotBeEmpty();
    }

    [Fact]
    public void AddApplication_registers_domain_event_handlers()
    {
        var services = new ServiceCollection();

        services.AddSharedKernel(
            new[] { typeof(SharedKernel.Tests.DependencyInjection.Fakes.AssemblyMarker).Assembly }
        );

        using var provider = services.BuildServiceProvider();

        var handlers = provider.GetServices<IDomainEventHandler<SharedKernel.Tests.DependencyInjection.Fakes.FakeDomainEvent>>();

        handlers.Should().NotBeEmpty();
    }
}
