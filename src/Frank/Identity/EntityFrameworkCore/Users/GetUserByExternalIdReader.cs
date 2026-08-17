using Frank.Identity.Application.Abstractions.Users.GetUserByExternalId;
using Frank.Identity.Domain.Users;
using Frank.Identity.EntityFrameworkCore.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Frank.Identity.EntityFrameworkCore.Users;

/// <summary>
/// EF Core implementation of <see cref="IGetUserByExternalIdReader"/> responsible
/// for retrieving a <see cref="User"/> by its external identity provider ID.
/// <para>
/// This reader is part of the vertical slice for external‑ID based user lookup.
/// It performs a read‑only query using <see cref="AsNoTracking"/> to avoid
/// unnecessary EF Core change tracking.
/// </para>
/// <para>
/// The reader returns a lightweight <see cref="GetUserByExternalIdResponse"/>
/// containing only the user’s internal Identity ID, which is sufficient for
/// upstream authentication and account‑linking flows.
/// </para>
/// </summary>
public sealed class GetUserByExternalIdReader : IGetUserByExternalIdReader
{
    private readonly FrankIdentityDbContext _db;

    /// <summary>
    /// Initializes a new <see cref="GetUserByExternalIdReader"/> using the provided
    /// <see cref="FrankIdentityDbContext"/>.
    /// </summary>
    /// <param name="db">The EF Core DbContext used to query user entities.</param>
    public GetUserByExternalIdReader(FrankIdentityDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves a user by its external identity provider ID.
    /// <para>
    /// The lookup matches <see cref="User.ExternalId"/> when present and returns
    /// a minimal response containing the internal <see cref="UserId"/> value.
    /// </para>
    /// </summary>
    /// <param name="externalId">The external identity provider ID to match.</param>
    /// <param name="ct">A cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="GetUserByExternalIdResponse"/> if a matching user exists;
    /// otherwise <c>null</c>.
    /// </returns>
    public Task<GetUserByExternalIdResponse?> ReadAsync(
        string externalId,
        CancellationToken ct)
    {
        return _db.Set<User>()
            .AsNoTracking()
            .Where(c =>
                c.ExternalId != null &&
                c.ExternalId.Value == externalId)
            .Select(c =>
                new GetUserByExternalIdResponse(
                    Id: c.Id.Value))
            .SingleOrDefaultAsync(ct);
    }
}
