using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Abstractions.DomainEvents;
using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Frank.Core.Application.Cqrs.Commands;
using Frank.Core.Application.DomainEvents;
using Frank.Core.Application.Cqrs.Queries;

using Frank.TestUtilities.ValidServices;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Tests.Registration;

public sealed class AutoRegistrationTests
{
    [Fact]
    public void AddApplication_registers_command_handlers()
    {
        var services = new ServiceCollection();

        services.AddFrankCqrsCommands(
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

        services.AddFrankCqrsQueries(
            [typeof(Frank.TestUtilities.ValidServices.AssemblyMarker).Assembly]
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
            new[] { typeof(Frank.TestUtilities.ValidServices.AssemblyMarker).Assembly }
        );

        using var provider = services.BuildServiceProvider();

        var validators = provider.GetServices<IValidator<Frank.TestUtilities.ValidServices.FakeCommand>>();

        validators.Should().NotBeEmpty();
    }

    [Fact]
    public void AddApplication_registers_domain_event_handlers()
    {
        var services = new ServiceCollection();

        services.AddFrankDomainEvents([
            typeof(Frank.TestUtilities.ValidServices.AssemblyMarker).Assembly
        ]);

        using var provider = services.BuildServiceProvider();

        var handlers = provider.GetServices<IDomainEventHandler<Frank.TestUtilities.ValidServices.FakeDomainEvent>>();

        handlers.Should().NotBeEmpty();
    }
}
