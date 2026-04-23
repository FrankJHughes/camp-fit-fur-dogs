using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions;

namespace SharedKernel.Infrastructure.EntityFrameworkCore;

public sealed class EfUnitOfWork<TContext> : IUnitOfWork
    where TContext : DbContext
{
    private readonly TContext _dbContext;

    public EfUnitOfWork(TContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        // 1. Save changes
        var result = await _dbContext.SaveChangesAsync(cancellationToken);

        return result;
    }
}
