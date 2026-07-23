using Frank.Core.Application.Abstractions.DomainEvents;
using Frank.Core.Application.DomainEvents;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Tests.Registration;

public sealed class AutoRegistration_DomainEventHandlerTests
{
    [Fact]
    public void AddApplication_registers_multiple_domain_event_handlers()
    {
        var services = new ServiceCollection();

        services.AddFrankCoreApplicationDomainEvents([
            typeof(Frank.TestUtilities.ValidServices.AssemblyMarker).Assembly
        ]);

        using var provider = services.BuildServiceProvider();

        var handlers = provider.GetServices<IDomainEventHandler<Frank.TestUtilities.ValidServices.FakeDomainEvent>>();

        handlers.Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public void AddApplication_does_not_register_abstract_domain_event_handlers()
    {
        var services = new ServiceCollection();

        services.AddValidatorsFromAssemblies(
            [typeof(Frank.TestUtilities.ValidServices.AssemblyMarker).Assembly]
        );

        using var provider = services.BuildServiceProvider();

        // Get all registered handlers for FakeDomainEvent
        var handlers = provider.GetServices<IDomainEventHandler<Frank.TestUtilities.ValidServices.FakeDomainEvent>>();

        // Assert that none of the registered handlers are abstract
        handlers.Should().OnlyContain(h => !h.GetType().IsAbstract);
    }
}
