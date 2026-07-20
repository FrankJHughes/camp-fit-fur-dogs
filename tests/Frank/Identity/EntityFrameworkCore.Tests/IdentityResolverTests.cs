using Frank.Identity.Application;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Users.CreateUser;
using Frank.Identity.Application.Abstractions.Users.GetUserByExternalId;
using Frank.Identity.Domain.Users;

namespace Frank.Core.EntityFrameworkCore.Tests.Identity;

public sealed class IdentityResolverTests
{
    // ------------------------------------------------------------
    // FAKES
    // ------------------------------------------------------------

    private sealed class FakeReader : IGetUserByExternalIdReader
    {
        public string? ReceivedExternalId { get; private set; }
        public GetUserByExternalIdResponse? Returned { get; set; }

        public Task<GetUserByExternalIdResponse?> GetByExternalIdAsync(
            string externalId,
            CancellationToken ct)
        {
            ReceivedExternalId = externalId;
            return Task.FromResult(Returned);
        }
    }

    private sealed class FakeWriter : ICreateUserWriter
    {
        public User? ReceivedUser { get; private set; }

        public Task WriteAsync(User user, CancellationToken ct)
        {
            ReceivedUser = user;
            return Task.CompletedTask;
        }
    }

    private static CallbackOidcContextBuilderResult External(
        string sub = "test0|sub-123",
        string given = "John",
        string family = "Doe",
        string email = "john@example.com")
        => new()
        {
            SubjectId = sub,
            Claims = new Dictionary<string, string>(),
            GivenName = given,
            FamilyName = family,
            Email = email
        };

    // ------------------------------------------------------------
    // TESTS
    // ------------------------------------------------------------

    [Fact]
    public async Task ResolveAsync_WhenUserExists_ReturnsExistingId()
    {
        var existingId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var reader = new FakeReader
        {
            Returned = new GetUserByExternalIdResponse(existingId)
        };

        var writer = new FakeWriter();
        var resolver = new IdentityResolver(reader, writer);

        var result = await resolver.ResolveAsync(External(), CancellationToken.None);

        result.Should().Be(existingId);
        reader.ReceivedExternalId.Should().Be("test0|sub-123");

        // Should NOT create a new user
        writer.ReceivedUser.Should().BeNull();
    }

    [Fact]
    public async Task ResolveAsync_WhenUserDoesNotExist_CreatesUser()
    {
        var reader = new FakeReader { Returned = null };
        var writer = new FakeWriter();

        var resolver = new IdentityResolver(reader, writer);

        var result = await resolver.ResolveAsync(External(), CancellationToken.None);

        // The resolver returns the new user's ID
        writer.ReceivedUser.Should().NotBeNull();
        result.Should().Be(writer.ReceivedUser!.Id.Value);

        var user = writer.ReceivedUser!;
        user.ExternalId.Value.Should().Be("test0|sub-123");
        user.FirstName.Value.Should().Be("John");
        user.LastName.Value.Should().Be("Doe");
        user.Email.Value.Should().Be("john@example.com");
    }

    [Fact]
    public async Task ResolveAsync_Throws_WhenCancellationRequested()
    {
        var reader = new FakeReader();
        var writer = new FakeWriter();
        var resolver = new IdentityResolver(reader, writer);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await resolver.ResolveAsync(External(), cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
