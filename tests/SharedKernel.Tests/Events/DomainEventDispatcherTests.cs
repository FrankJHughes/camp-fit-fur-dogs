using FluentAssertions;
using SharedKernel.Events;
using SharedKernel.Tests.Fakes;
using Xunit;

namespace SharedKernel.Tests.Events;

public sealed class DomainEventDispatcherTests
{
    private sealed class TestEvent : IDomainEvent { }

    private sealed class TrackingHandler : IDomainEventHandler<TestEvent>
    {
        public List<TestEvent> Received { get; } = new();

        public Task Handle(TestEvent domainEvent, CancellationToken cancellationToken)
        {
            Received.Add(domainEvent);
            return Task.CompletedTask;
        }
    }

    private sealed class ThrowingHandler : IDomainEventHandler<TestEvent>
    {
        public Task Handle(TestEvent domainEvent, CancellationToken cancellationToken)
            => throw new InvalidOperationException("boom");
    }

    [Fact]
    public async Task DispatchAsync_Invokes_Single_Handler()
    {
        var evt = new TestEvent();
        var handler = new TrackingHandler();

        var sp = new FakeServiceProvider();
        sp.AddHandler<IDomainEventHandler<TestEvent>>(handler);

        var dispatcher = new DomainEventDispatcher(sp);

        await dispatcher.DispatchAsync(evt);

        handler.Received.Should().ContainSingle().Which.Should().Be(evt);
    }

    [Fact]
    public async Task DispatchAsync_Invokes_Multiple_Handlers()
    {
        var evt = new TestEvent();
        var h1 = new TrackingHandler();
        var h2 = new TrackingHandler();

        var sp = new FakeServiceProvider();
        sp.AddHandler<IDomainEventHandler<TestEvent>>(h1);
        sp.AddHandler<IDomainEventHandler<TestEvent>>(h2);

        var dispatcher = new DomainEventDispatcher(sp);

        await dispatcher.DispatchAsync(evt);

        h1.Received.Should().ContainSingle().Which.Should().Be(evt);
        h2.Received.Should().ContainSingle().Which.Should().Be(evt);
    }

    [Fact]
    public async Task DispatchAsync_Propagates_Exceptions()
    {
        var evt = new TestEvent();
        var handler = new ThrowingHandler();

        var sp = new FakeServiceProvider();
        sp.AddHandler<IDomainEventHandler<TestEvent>>(handler);

        var dispatcher = new DomainEventDispatcher(sp);

        Func<Task> act = () => dispatcher.DispatchAsync(evt);

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("boom");
    }

    [Fact]
    public async Task DispatchAsync_No_Handlers_Does_Not_Throw()
    {
        var evt = new TestEvent();
        var sp = new FakeServiceProvider();
        var dispatcher = new DomainEventDispatcher(sp);

        Func<Task> act = () => dispatcher.DispatchAsync(evt);

        await act.Should().NotThrowAsync();
    }
}
