using Frank.Identity.Application.Abstractions.Users.GetUserById;
using Frank.Identity.Domain.Users;
using Frank.Identity.EntityFrameworkCore.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Frank.Identity.EntityFrameworkCore.Users;

/// <summary>
/// EF Core implementation of <see cref="IGetUserByIdReader"/> responsible for
/// retrieving a <see cref="User"/> aggregate by its internal Identity ID.
/// <para>
/// This reader is part of the vertical slice for user lookup by ID. It performs
/// a read‑only query using <see cref="AsNoTracking"/> to avoid unnecessary EF Core
/// change tracking, returning a lightweight response containing only the fields
/// required by the application layer.
/// </para>
/// </summary>
public sealed class GetUserByIdReader : IGetUserByIdReader
{
    private readonly FrankIdentityDbContext _db;

    /// <summary>
    /// Initializes a new <see cref="GetUserByIdReader"/> using the provided
    /// <see cref="FrankIdentityDbContext"/>.
    /// </summary>
    /// <param name="db">The EF Core DbContext used to query user entities.</param>
    public GetUserByIdReader(FrankIdentityDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves a user by its internal Identity ID.
    /// <para>
    /// The lookup matches the <see cref="UserId"/> value object and returns a
    /// <see cref="GetUserByIdResponse"/> containing the user’s ID and basic profile
    /// information (first and last name).
    /// </para>
    /// </summary>
    /// <param name="userId">The internal Identity user ID to match.</param>
    /// <param name="ct">A cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="GetUserByIdResponse"/> if a matching user exists; otherwise <c>null</c>.
    /// </returns>
    public Task<GetUserByIdResponse?> ReadAsync(
        Guid userId,
        CancellationToken ct)
    {
        return _db.Set<User>()
            .AsNoTracking()
            .Where(c =>
                c.Id == UserId.From(userId))
            .Select(c =>
                new GetUserByIdResponse(
                    Id: c.Id.Value,
                    FirstName: c.FirstName.Value,
                    LastName: c.LastName.Value))
            .SingleOrDefaultAsync(ct);
    }
}
