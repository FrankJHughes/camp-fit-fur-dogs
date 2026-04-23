using FluentAssertions;
using SharedKernel.Events;
using Xunit;

namespace SharedKernel.Tests.Events;

public sealed class DomainEventHandlerTests
{
    private sealed class TestEvent : IDomainEvent { }

    private sealed class TrackingHandler : IDomainEventHandler<TestEvent>
    {
        public int CallCount { get; private set; }
        public TestEvent? LastEvent { get; private set; }
        public CancellationToken? LastToken { get; private set; }

        public Task Handle(TestEvent domainEvent, CancellationToken cancellationToken)
        {
            CallCount++;
            LastEvent = domainEvent;
            LastToken = cancellationToken;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Handler_Receives_Event()
    {
        var evt = new TestEvent();
        var handler = new TrackingHandler();

        await handler.Handle(evt, CancellationToken.None);

        handler.CallCount.Should().Be(1);
        handler.LastEvent.Should().Be(evt);
    }

    [Fact]
    public async Task Handler_Receives_CancellationToken()
    {
        var evt = new TestEvent();
        var handler = new TrackingHandler();
        var token = new CancellationTokenSource().Token;

        await handler.Handle(evt, token);

        handler.LastToken.Should().Be(token);
    }
}
