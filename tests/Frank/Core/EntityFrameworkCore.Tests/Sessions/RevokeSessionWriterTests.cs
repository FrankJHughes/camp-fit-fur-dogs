using Frank.Identity.Domain.Sessions;
using Frank.Identity.Domain.Users;
using Frank.Identity.EntityFrameworkCore.DbContexts;
using Frank.Identity.EntityFrameworkCore.Sessions;
using Frank.Identity.EntityFrameworkCore.Users;
using Frank.TestUtilities.Builders;
using Frank.TestUtilities.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Frank.Core.EntityFrameworkCore.Tests.Sessions;

public class RevokeSessionWriterTests : IClassFixture<PostgresFixture<FrankIdentityDbContext>>
{
    private readonly PostgresFixture<FrankIdentityDbContext> _fixture;

    public RevokeSessionWriterTests(PostgresFixture<FrankIdentityDbContext> fixture)
    {
        _fixture = fixture;
    }

    private async Task<UserId> SeedOwnerAsync()
    {
        await using var ctx = _fixture.CreateContext();
        var writer = new CreateUserWriter(ctx);

        var user = new UserBuilder()
            .WithFirstName(UserFixtures.First.Value)
            .WithLastName(UserFixtures.Last.Value)
            .WithEmail($"infra-{Guid.NewGuid()}@example.com")
            .WithPhone(UserFixtures.Phone.Value)
            .Build();

        await writer.WriteAsync(user, CancellationToken.None);
        await ctx.SaveChangesAsync();

        return user.Id;
    }

    [Fact]
    public async Task RevokeAsync_marks_session_as_revoked()
    {
        var ownerId = await SeedOwnerAsync();

        SessionTokenHash tokenHash;

        // Arrange — create session
        await using (var ctx = _fixture.CreateContext())
        {
            var writer = new CreateSessionWriter(ctx);

            tokenHash = SessionTokenHash.From(
                Guid.NewGuid().ToString("N").PadLeft(64, 'd')
            );

            var session = new SessionBuilder()
                .WithOwner(ownerId)
                .WithTokenHash(tokenHash)
                .CreatedAtFromFixture()
                .Build();

            await writer.WriteAsync(session, CancellationToken.None);
            await ctx.SaveChangesAsync();
        }

        // Act — revoke
        var before = DateTimeOffset.UtcNow;

        await using (var ctx = _fixture.CreateContext())
        {
            var writer = new RevokeSessionWriter(ctx);
            await writer.WriteAsync(tokenHash, CancellationToken.None);
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
