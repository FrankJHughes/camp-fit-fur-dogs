using Frank.Identity.Domain.Users;

namespace Frank.Identity.Application.Abstractions.Users.CreateUser;

/// <summary>
/// Defines the contract for persisting a newly created <see cref="User"/> entity
/// within the Identity subsystem.
/// <para>
/// This writer is part of the user‑creation pipeline. It is responsible for
/// storing the newly constructed domain <c>User</c> in durable storage after the
/// <see cref="CreateUserCommand"/> has been validated and transformed into a
/// domain entity.
/// </para>
/// <para>
/// The writer abstracts away all infrastructure concerns such as database access,
/// unique constraints, transactional guarantees, and indexing.
/// The application layer depends only on this interface, never on persistence
/// details.
/// </para>
/// </summary>
/// <remarks>
/// Implementations may enforce:
/// <list type="bullet">
/// <item><description>Email uniqueness constraints</description></item>
/// <item><description>External identity provider ID uniqueness</description></item>
/// <item><description>Transactional boundaries via <c>IFrankIdentityUnitOfWork</c></description></item>
/// <item><description>Normalization rules (email, phone)</description></item>
/// </list>
/// The writer must persist the <c>User</c> entity exactly as provided by the
/// command handler, without injecting additional domain logic.
/// </remarks>
public interface ICreateUserWriter
{
    /// <summary>
    /// Persists the provided <see cref="User"/> entity to durable storage.
    /// <para>
    /// Implementations must ensure atomicity, consistency, and correct handling
    /// of uniqueness constraints. If the user cannot be written due to a
    /// constraint violation, the implementation should throw a domain‑appropriate
    /// exception.
    /// </para>
    /// </summary>
    /// <param name="user">
    /// The fully constructed domain <see cref="User"/> entity to be persisted.
    /// </param>
    /// <param name="cancellationToken">
    /// A token allowing the caller to cancel the write operation.
    /// </param>
    Task WriteAsync(User user, CancellationToken cancellationToken);
}
