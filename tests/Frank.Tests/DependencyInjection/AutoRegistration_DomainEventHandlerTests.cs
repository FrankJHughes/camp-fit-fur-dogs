using Frank.Abstractions.Events;
using Frank.DependencyInjection;
using Frank.Events;

namespace Frank.Tests.DependencyInjection;

public sealed class AutoRegistration_DomainEventHandlerTests
{
    [Fact]
    public void AddApplication_registers_multiple_domain_event_handlers()
    {
        var services = new ServiceCollection();

        services.AddFrank(
            [typeof(Frank.Tests.DependencyInjection.Fakes.AssemblyMarker).Assembly]
        );

        using var provider = services.BuildServiceProvider();

        var handlers = provider.GetServices<IDomainEventHandler<Frank.Tests.DependencyInjection.Fakes.FakeDomainEvent>>();

        handlers.Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public void AddApplication_does_not_register_abstract_domain_event_handlers()
    {
        var services = new ServiceCollection();

        services.AddFrank(
            new[] { typeof(Frank.Tests.DependencyInjection.Fakes.AssemblyMarker).Assembly }
        );

        using var provider = services.BuildServiceProvider();

        // Get all registered handlers for FakeDomainEvent
        var handlers = provider.GetServices<IDomainEventHandler<Frank.Tests.DependencyInjection.Fakes.FakeDomainEvent>>();

        // Assert that none of the registered handlers are abstract
        handlers.Should().OnlyContain(h => !h.GetType().IsAbstract);
    }
}
