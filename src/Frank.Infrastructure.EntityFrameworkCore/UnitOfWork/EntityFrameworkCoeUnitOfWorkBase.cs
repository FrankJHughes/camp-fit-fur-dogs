using Frank.Abstractions.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Frank.Infrastructure.EntityFrameworkCore;

public class EntityFrameworkCoreUnitOfWorkBase<TContext> : IUnitOfWork
    where TContext : DbContext
{
    private readonly TContext _dbContext = default!;

    public EntityFrameworkCoreUnitOfWorkBase(TContext dbContext)
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
