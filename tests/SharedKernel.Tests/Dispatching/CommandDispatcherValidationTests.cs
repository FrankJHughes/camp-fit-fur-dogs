
using SharedKernel.Tests.Fakes;

namespace SharedKernel.Tests.Dispatching;

public class CommandDispatcherValidationTests : DispatcherTestBase
{
    public CommandDispatcherValidationTests()
    {
        WithDispatcher<SharedKernel.CommandDispatcher, ICommandDispatcher>();
    }

    [Fact]
    public async Task Dispatch_InvalidCommand_Fails_Validation_And_Does_Not_Invoke_Handler()
    {
        var handler = new TrackingCommandHandler<SendMessageCommand, SendMessageResponse>();

        WithInstance<ICommandHandler<SendMessageCommand, SendMessageResponse>>(handler);
        WithValidator<SendMessageCommand, FailingValidator<SendMessageCommand>>();
        BuildContainer();

        var dispatcher = Provider.GetRequiredService<ICommandDispatcher>();

        await Assert.ThrowsAsync<ValidationException>(() =>
            dispatcher.DispatchAsync(
                new SendMessageCommand("bad"),
                CancellationToken.None));

        handler.CallCount.Should().Be(0);
    }
}

