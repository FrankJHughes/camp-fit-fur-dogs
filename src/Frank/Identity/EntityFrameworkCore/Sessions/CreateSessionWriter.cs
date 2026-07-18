using Frank.Identity.Application.Abstractions.Sessions.CreateSession;
using Frank.Identity.Domain.Sessions;
using Frank.Identity.EntityFrameworkCore.DbContexts;

namespace Frank.Identity.EntityFrameworkCore.Sessions;

public sealed class CreateSessionWriter : ICreateSessionWriter
{
    private readonly FrankIdentityDbContext _db;

    public CreateSessionWriter(FrankIdentityDbContext db)
    {
        _db = db;
    }

    public Task WriteAsync(Session session, CancellationToken cancellationToken)
    {
        _db.Set<Session>().Add(session);
        return Task.CompletedTask;
    }

}
