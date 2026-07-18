
using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application.Abstractions.Sessions.CreateSession;

public interface ICreateSessionWriter
{
    Task WriteAsync(Session session, CancellationToken ct);
}
