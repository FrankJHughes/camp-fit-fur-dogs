using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace Frank.Identity.Application.Abstractions.Sessions.GetSession;

/// <summary>
/// Represents a query used to retrieve a session by its token hash.
/// <para>
/// This query is part of the Identity subsystem’s session‑retrieval pipeline.
/// It is issued when the application needs to resolve an authenticated session
/// based on a token presented by the client (typically via a cookie or header).
/// </para>
/// <para>
/// The token hash is a secure, non‑reversible representation of the session
/// token. Only the hash is stored in persistent storage; the raw token is never
/// persisted.
/// </para>
/// </summary>
/// <remarks>
/// This query follows the CQRS pattern and returns a
/// <see cref="GetSessionResponse"/> when the session exists, or <c>null</c>
/// when no matching session is found.
/// The query handler is responsible for performing lookup, validation, and any
/// expiration or revocation checks.
/// </remarks>
public record GetSessionQuery(string TokenHash) : IQuery<GetSessionResponse?>;
