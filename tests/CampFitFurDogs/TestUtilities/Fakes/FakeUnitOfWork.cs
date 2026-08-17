using CampFitFurDogs.Application.Abstractions.UnitOfWork;

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
