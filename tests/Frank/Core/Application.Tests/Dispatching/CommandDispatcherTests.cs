
using Frank.Core.Application.Abstractions.Command;
using Frank.Core.Application.Command;
using Frank.Core.Application.Tests.Fakes;
using Frank.Core.Application.Tests.Slices;
using Frank.Core.Application.Tests.TestInfrastructure;
using Frank.Core.Application.Tests.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Tests.Dispatching;

public class CommandDispatcherTests : DispatcherTestBase
{
    public CommandDispatcherTests()
    {
        WithDispatcher<CommandDispatcher, ICommandDispatcher>();
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

