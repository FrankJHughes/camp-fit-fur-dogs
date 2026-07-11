using Frank.Domain.Sessions;
using Frank.Domain.Users;
using Frank.Infrastructure.EntityFrameworkCore.Sessions;
using Frank.Infrastructure.EntityFrameworkCore.Users;
using Frank.TestUtilities.Builders;
using Frank.TestUtilities.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Frank.Infrastructure.EntityFrameworkCore.Persistence;
using Xunit;

namespace Frank.Infrastructure.EntityFrameworkCore.Tests.Sessions;

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

        await repo.CreateAsync(session);
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
    public async Task GetByTokenHashAsync_existing_session_returns_session()
    {
        var ownerId = await SeedOwnerAsync();

        SessionTokenHash tokenHash;

        await using (var ctx = _fixture.CreateContext())
        {
            var repo = new SessionRepository(ctx);

            tokenHash = SessionTokenHash.From(
                Guid.NewGuid().ToString("N").PadLeft(64, 'b')
            );

            var session = new SessionBuilder()
                .WithOwner(ownerId)
                .WithTokenHash(tokenHash)
                .CreatedAtFromFixture()
                .Build();

            await repo.CreateAsync(session);
            await ctx.SaveChangesAsync();
        }

        await using var readCtx = _fixture.CreateContext();
        var readRepo = new SessionRepository(readCtx);

        var retrieved = await readRepo.GetByTokenHashAsync(tokenHash);

        retrieved.Should().NotBeNull();
        retrieved!.TokenHash.Should().Be(tokenHash);
        retrieved.OwnerId.Should().Be(ownerId);
        retrieved.CreatedAt.Should().Be(SessionFixtures.CreatedAt);
        retrieved.RevokedAt.Should().BeNull();
    }

    [Fact]
    public async Task GetByTokenHashAsync_nonexistent_hash_returns_null()
    {
        await using var ctx = _fixture.CreateContext();
        var repo = new SessionRepository(ctx);

        var missingHash = SessionTokenHash.From(
            Guid.NewGuid().ToString("N").PadLeft(64, 'c')
        );

        var result = await repo.GetByTokenHashAsync(missingHash);

        result.Should().BeNull();
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

            await repo.CreateAsync(session);
            await ctx.SaveChangesAsync();
        }

        // Act — revoke
        var before = DateTimeOffset.UtcNow;

        await using (var ctx = _fixture.CreateContext())
        {
            var repo = new SessionRepository(ctx);
            await repo.RevokeAsync(tokenHash);
            await ctx.SaveChangesAsync();
        }

        // Assert — session still exists but is revoked
        await using (var readCtx = _fixture.CreateContext())
        {
            var repo = new SessionRepository(readCtx);
            var retrieved = await repo.GetByTokenHashAsync(tokenHash);

            retrieved.Should().NotBeNull();
            retrieved!.RevokedAt.Should().NotBeNull();
            retrieved.RevokedAt.Should().BeOnOrAfter(before);
        }
    }
}
