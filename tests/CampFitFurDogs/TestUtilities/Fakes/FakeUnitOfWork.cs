using CampFitFurDogs.Application.Abstractions;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeAppUnitOfWork : IAppUnitOfWork
{
    public int CommitCount { get; private set; }

    public Task<int> CommitAsync(CancellationToken ct = default)
    {
        CommitCount++;
        return Task.FromResult(1);
    }
}
