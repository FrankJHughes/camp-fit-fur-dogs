namespace SharedKernel.Abstractions;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
