
using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application.Abstractions.Sessions.RevokeSession;

public interface IRevokeSessionWriter
{
    Task WriteAsync(
        SessionTokenHash tokenHash, CancellationToken ct);
}
