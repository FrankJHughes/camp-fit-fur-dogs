using Frank.Identity.EntityFrameworkCore.Sessions;
using Frank.Identity.Domain.Sessions;
using Frank.Identity.Domain.Users;
using Frank.Identity.EntityFrameworkCore.Persistence;
using Frank.Identity.EntityFrameworkCore.Users;
using Frank.TestUtilities.Builders;
using Frank.TestUtilities.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Frank.Core.EntityFrameworkCore.Tests.Sessions;

public class SessionRepositoryTests : IClassFixture<PostgresFixture<FrankIdentityDbContext>>
{
    private readonly PostgresFixture<FrankIdentityDbContext> _fixture;

    public SessionRepositoryTests(PostgresFixture<FrankIdentityDbContext> fixture)
    {
        _fixture = fixture;
    }

    private async Task<UserId> SeedOwnerAsync()
    {
        await using var ctx = _fixture.CreateContext();
        var repo = new UserRepository(ctx);

        var user = new UserBuilder()
            .WithFirstName(UserFixtures.First.Value)
            .WithLastName(UserFixtures.Last.Value)
            .WithEmail($"infra-{Guid.NewGuid()}@example.com")
            .WithPhone(UserFixtures.Phone.Value)
            .Build();

        await repo.AddAsync(user, CancellationToken.None);
        await ctx.SaveChangesAsync();

        return user.Id;
    }

    [Fact]
    public async Task CreateAsync_persists_session_with_correct_mapping()
    {
        var ownerId = await SeedOwnerAsync();

        await using var ctx = _fixture.CreateContext();
        var repo = new SessionRepository(ctx);

        var session = new SessionBuilder()
            .WithOwner(ownerId)
            .WithRandomTokenHash()
            .CreatedAtFromFixture()
            .Build();

        await repo.CreateAsync(session, CancellationToken.None);
        await ctx.SaveChangesAsync();

        await using var readCtx = _fixture.CreateContext();
        var persisted = await readCtx.Set<Session>()
            .AsNoTracking()
            .SingleAsync(s => s.Id == session.Id);

        persisted.TokenHash.Should().Be(session.TokenHash);
        persisted.OwnerId.Should().Be(ownerId);
        persisted.CreatedAt.Should().Be(SessionFixtures.CreatedAt);
        persisted.RevokedAt.Should().BeNull();
    }

    [Fact]
    public async Task RevokeAsync_marks_session_as_revoked()
    {
        var ownerId = await SeedOwnerAsync();

        SessionTokenHash tokenHash;

        // Arrange — create session
        await using (var ctx = _fixture.CreateContext())
        {
            var repo = new SessionRepository(ctx);

            tokenHash = SessionTokenHash.From(
                Guid.NewGuid().ToString("N").PadLeft(64, 'd')
            );

            var session = new SessionBuilder()
                .WithOwner(ownerId)
                .WithTokenHash(tokenHash)
                .CreatedAtFromFixture()
                .Build();

            await repo.CreateAsync(session, CancellationToken.None);
            await ctx.SaveChangesAsync();
        }

        // Act — revoke
        var before = DateTimeOffset.UtcNow;

        await using (var ctx = _fixture.CreateContext())
        {
            var repo = new SessionRepository(ctx);
            await repo.RevokeAsync(tokenHash, CancellationToken.None);
            await ctx.SaveChangesAsync();
        }

        // Assert — session still exists but is revoked
        await using (var readCtx = _fixture.CreateContext())
        {
            var retrieved = await readCtx
                .Set<Session>()
                .FirstOrDefaultAsync(s => s.TokenHash == tokenHash);

            retrieved.Should().NotBeNull();
            retrieved!.RevokedAt.Should().NotBeNull();
            retrieved.RevokedAt.Should().BeOnOrAfter(before);
        }
    }
}
