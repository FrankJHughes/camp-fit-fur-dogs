namespace Frank.Core.Application.Abstractions.UnitOfWork;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
