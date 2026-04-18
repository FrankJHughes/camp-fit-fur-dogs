using System.Threading;
using System.Threading.Tasks;
using CampFitFurDogs.Application.Abstractions;

namespace TestDoubles.Dispatchers;

public sealed class TestCommandHandler : ICommandHandler<TestCommand, string>
{
    public bool WasExecuted { get; private set; }
    public TestCommand? Received { get; private set; }

    public Task<string> Handle(TestCommand command, CancellationToken ct)
    {
        WasExecuted = true;
        Received = command;
        return Task.FromResult("OK");
    }
}
