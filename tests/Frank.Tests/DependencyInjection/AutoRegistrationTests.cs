using Frank.Abstractions.Command;
using Frank.Abstractions.Event;
using Frank.Abstractions.Query;
using Frank.Command;
using Frank.Event;
using Frank.Query;

using Frank.Tests.DependencyInjection.Fakes;

namespace Frank.Tests.DependencyInjection;

public sealed class AutoRegistrationTests
{
    [Fact]
    public void AddApplication_registers_command_handlers()
    {
        var services = new ServiceCollection();

        services.AddFrankCommand(
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

        services.AddFrankQuery(
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

        services.AddFrank(
            new[] { typeof(Frank.Tests.DependencyInjection.Fakes.AssemblyMarker).Assembly }
        );

        using var provider = services.BuildServiceProvider();

        var validators = provider.GetServices<IValidator<Frank.Tests.DependencyInjection.Fakes.FakeCommand>>();

        validators.Should().NotBeEmpty();
    }

    [Fact]
    public void AddApplication_registers_domain_event_handlers()
    {
        var services = new ServiceCollection();

        services.AddFrankEvent([
            typeof(Frank.Tests.DependencyInjection.Fakes.AssemblyMarker).Assembly
        ]);

        using var provider = services.BuildServiceProvider();

        var handlers = provider.GetServices<IEventHandler<Frank.Tests.DependencyInjection.Fakes.FakeDomainEvent>>();

        handlers.Should().NotBeEmpty();
    }
}
