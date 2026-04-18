using CampFitFurDogs.Application.Abstractions;

namespace CampFitFurDogs.Infrastructure.Data;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public EfUnitOfWork(AppDbContext db)
    {
        _db = db;
    }

    public Task<int> CommitAsync(CancellationToken ct = default)
    {
        return _db.SaveChangesAsync(ct);
    }
}
