using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Frank.Identity.Application.Abstractions.Sessions.GetSession;
using Frank.Identity.Domain.Sessions.Errors;

namespace Frank.Identity.Application.Sessions.GetSession;

/// <summary>
/// Handles a <see cref="GetSessionQuery"/> by retrieving a session using its
/// token hash and returning the corresponding <see cref="GetSessionResponse"/>.
/// <para>
/// This query handler is part of the session‑retrieval flow and is typically
/// used during authentication, cookie validation, or session introspection.
/// </para>
/// <para>
/// If no session is found for the provided token hash, a
/// <see cref="SessionNotFoundException"/> is thrown.
/// </para>
/// </summary>
public sealed class GetSessionByIdHandler(IGetSessionReader reader)
    : IQueryHandler<GetSessionQuery, GetSessionResponse?>
{
    /// <summary>
    /// Executes the query by reading the session associated with the provided
    /// token hash.
    /// </summary>
    /// <param name="query">
    /// The query containing the token hash used to locate the session.
    /// </param>
    /// <param name="ct">
    /// A cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="GetSessionResponse"/> if the session exists; otherwise
    /// <c>null</c>.
    /// </returns>
    /// <exception cref="SessionNotFoundException">
    /// Thrown when no session exists for the provided token hash.
    /// </exception>
    public async Task<GetSessionResponse?> HandleAsync(
        GetSessionQuery query, CancellationToken ct)
    {
        var result = await reader.ReadAsync(query.TokenHash, ct)
            ?? throw new SessionNotFoundException();

        return result;
    }
}
