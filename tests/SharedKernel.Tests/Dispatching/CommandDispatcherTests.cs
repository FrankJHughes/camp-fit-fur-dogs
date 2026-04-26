
using SharedKernel.Tests.Fakes;

namespace SharedKernel.Tests.Dispatching;

public class CommandDispatcherTests : DispatcherTestBase
{
    public CommandDispatcherTests()
    {
        WithDispatcher<SharedKernel.CommandDispatcher, ICommandDispatcher>();
    }

    [Fact]
    public async Task Dispatch_SendMessageCommand_Returns_Response()
    {
        WithCommandHandler<SendMessageCommand, SendMessageResponse, SendMessageCommandHandler>();
        WithValidator<SendMessageCommand, SendMessageCommandValidator>();
        BuildContainer();

        var dispatcher = Provider.GetRequiredService<ICommandDispatcher>();

        var response = await dispatcher.DispatchAsync(
            new SendMessageCommand("hello"),
            CancellationToken.None);

        response.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Dispatch_VoidCommand_Completes_Without_Exception()
    {
        WithCommandHandler<DeleteMessageCommand, DeleteMessageCommandHandler>();
        BuildContainer();

        var dispatcher = Provider.GetRequiredService<ICommandDispatcher>();

        await dispatcher.DispatchAsync(
            new DeleteMessageCommand("msg-123"),
            CancellationToken.None);
    }
}

