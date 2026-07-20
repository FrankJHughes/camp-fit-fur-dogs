
namespace Frank.Identity.Application.Abstractions.Sessions.GetSession;

public interface IGetSessionReader
{
    Task<GetSessionResponse?> ReadAsync(
        string tokenHash, CancellationToken ct);
}
