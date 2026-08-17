using Frank.Identity.Application.Abstractions.Users.CreateUser;
using Frank.Identity.Domain.Users;
using Frank.Identity.EntityFrameworkCore.DbContexts;

namespace Frank.Identity.EntityFrameworkCore.Users;

/// <summary>
/// EF Core implementation of <see cref="ICreateUserWriter"/> responsible for
/// persisting newly created <see cref="User"/> aggregates.
/// <para>
/// This writer is part of the vertical slice for user creation. It attaches the
/// <see cref="User"/> entity to the <see cref="FrankIdentityDbContext"/> so that
/// it can be committed as part of the unit‑of‑work transaction.
/// </para>
/// <para>
/// The writer does not call <c>SaveChangesAsync</c>; transactional boundaries are
/// coordinated by <see cref="IFrankIdentityUnitOfWork"/> in the application layer.
/// </para>
/// </summary>
public sealed class CreateUserWriter : ICreateUserWriter
{
    private readonly FrankIdentityDbContext _db;

    /// <summary>
    /// Initializes a new <see cref="CreateUserWriter"/> using the provided
    /// <see cref="FrankIdentityDbContext"/>.
    /// </summary>
    /// <param name="db">The EF Core DbContext used to persist user entities.</param>
    public CreateUserWriter(FrankIdentityDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Persists a newly created <see cref="User"/> aggregate by adding it to the
    /// EF Core change tracker.
    /// <para>
    /// The caller is responsible for committing the transaction via the unit of work.
    /// </para>
    /// </summary>
    /// <param name="user">The user aggregate to persist.</param>
    /// <param name="ct">A cancellation token for the operation.</param>
    public async Task WriteAsync(User user, CancellationToken ct)
    {
        await _db.Set<User>().AddAsync(user, ct);
    }
}
