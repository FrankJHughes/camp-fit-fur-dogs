using Frank.Abstractions.UnitOfWork;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeUnitOfWork : IUnitOfWork
{
    public int CommitCount { get; private set; }

    public Task<int> CommitAsync(CancellationToken ct = default)
    {
        CommitCount++;
        return Task.FromResult(1);
    }
}
