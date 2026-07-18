using Frank.Identity.EntityFrameworkCore.Sessions;
using Frank.Identity.Domain.Sessions;
using Frank.Identity.Domain.Users;
using Frank.Identity.EntityFrameworkCore.DbContexts;
using Frank.TestUtilities.Builders;
using Frank.TestUtilities.Fixtures;
using Microsoft.EntityFrameworkCore;
using Frank.Identity.EntityFrameworkCore.Users;

namespace Frank.Core.EntityFrameworkCore.Tests.Sessions;

public class CreateSessionWriterTests : IClassFixture<PostgresFixture<FrankIdentityDbContext>>
{
    private readonly PostgresFixture<FrankIdentityDbContext> _fixture;

    public CreateSessionWriterTests(PostgresFixture<FrankIdentityDbContext> fixture)
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
    public async Task CreateAsync_persists_session_with_correct_mapping()
    {
        var ownerId = await SeedOwnerAsync();

        await using var ctx = _fixture.CreateContext();
        var repo = new CreateSessionWriter(ctx);

        var session = new SessionBuilder()
            .WithOwner(ownerId)
            .WithRandomTokenHash()
            .CreatedAtFromFixture()
            .Build();

        await repo.WriteAsync(session, CancellationToken.None);
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

}
