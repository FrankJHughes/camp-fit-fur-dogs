using Frank.Core.Application.Abstractions.UnitOfWork;

namespace Frank.TestUtilities.Fakes;

public class FakeFrankIdentityUnitOfWork : IFrankIdentityUnitOfWork
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
