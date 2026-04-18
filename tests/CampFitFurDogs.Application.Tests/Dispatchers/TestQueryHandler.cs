using System.Threading;
using System.Threading.Tasks;
using CampFitFurDogs.Application.Abstractions;

namespace TestDoubles.Dispatchers;

public sealed class TestQueryHandler : IQueryHandler<TestQuery, string>
{
    public bool WasExecuted { get; private set; }
    public TestQuery? Received { get; private set; }

    public Task<string> Handle(TestQuery query, CancellationToken ct)
    {
        WasExecuted = true;
        Received = query;
        return Task.FromResult("OK");
    }
}
