using Frank.Core.Application;
using Frank.Core.Application.Abstractions.Command;
using Frank.Core.Application.Abstractions.DomainEvents;
using Frank.Core.Application.Abstractions.Query;
using Frank.Core.Application.Command;
using Frank.Core.Application.DomainEvents;
using Frank.Core.Application.Query;

using Frank.Tests.DependencyInjection.Fakes;

namespace Frank.Tests.DependencyInjection;

public sealed class AutoRegistrationTests
{
    [Fact]
    public void AddApplication_registers_command_handlers()
    {
        var services = new ServiceCollection();

        services.AddFrankCommands(
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

        services.AddFrankValidators(
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

        services.AddFrankDomainEvents([
            typeof(Frank.Tests.DependencyInjection.Fakes.AssemblyMarker).Assembly
        ]);

        using var provider = services.BuildServiceProvider();

        var handlers = provider.GetServices<IDomainEventHandler<Frank.Tests.DependencyInjection.Fakes.FakeDomainEvent>>();

        handlers.Should().NotBeEmpty();
    }
}
