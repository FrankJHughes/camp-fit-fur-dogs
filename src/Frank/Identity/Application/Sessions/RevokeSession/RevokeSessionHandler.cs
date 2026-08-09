using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Identity.Application.Abstractions.Sessions.RevokeSession;
using Frank.Identity.Application.Abstractions.UnitOfWork;
using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application.Sessions.RevokeSession;

/// <summary>
/// Handles a <see cref="RevokeSessionCommand"/> by revoking an authenticated
/// session using its token hash.
/// <para>
/// This command handler is part of the session‑management flow and is typically
/// used during logout, forced session invalidation, or administrative session
/// revocation.
/// </para>
/// <para>
/// The handler converts the provided token hash into a domain
/// <see cref="SessionTokenHash"/>, writes the revocation using
/// <see cref="IRevokeSessionWriter"/>, and commits the unit of work.
/// </para>
/// </summary>
public sealed class RevokeSessionHandler : ICommandHandler<RevokeSessionCommand>
{
    private readonly IRevokeSessionWriter _writer;
    private readonly IFrankIdentityUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new <see cref="RevokeSessionHandler"/> using the provided
    /// session‑revocation writer and unit of work.
    /// </summary>
    /// <param name="writer">
    /// The writer responsible for persisting session revocation operations.
    /// </param>
    /// <param name="unitOfWork">
    /// The unit of work used to commit the revocation to the underlying store.
    /// </param>
    public RevokeSessionHandler(IRevokeSessionWriter writer, IFrankIdentityUnitOfWork unitOfWork)
    {
        _writer = writer;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Executes the command by revoking the session associated with the provided
    /// token hash and committing the change.
    /// </summary>
    /// <param name="command">
    /// The command containing the token hash of the session to revoke.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>A completed task once the session has been revoked.</returns>
    public async Task HandleAsync(RevokeSessionCommand command, CancellationToken cancellationToken)
    {
        var tokenHash = SessionTokenHash.From(command.TokenHash);

        await _writer.WriteAsync(tokenHash, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
