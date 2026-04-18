using CampFitFurDogs.Application.Abstractions;

namespace CampFitFurDogs.Application.Tests.Fakes;

public class FakeUnitOfWork : IUnitOfWork
{
    public bool Committed { get; private set; }
    public int CommitCount { get; private set; }

    public Task<int> CommitAsync(CancellationToken ct = default)
    {
        Committed = true;
        CommitCount++;
        return Task.FromResult(1);
    }
}
