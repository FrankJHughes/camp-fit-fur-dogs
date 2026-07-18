using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Users.CreateUser;
using Frank.Identity.Application.Abstractions.Users.GetUserByExternalId;
using Frank.Identity.Infrastructure;

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

    private sealed class FakeDispatcher : ICommandDispatcher
    {
        public object? ReceivedCommand { get; private set; }
        public Guid ReturnedId { get; set; } =
            Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        public Task<TResult> DispatchAsync<TResult>(
            ICommand<TResult> command,
            CancellationToken ct)
        {
            ReceivedCommand = command;
            return Task.FromResult((TResult)(object)ReturnedId);
        }

        public Task DispatchAsync(ICommand command, CancellationToken ct)
        {
            ReceivedCommand = command;
            return Task.CompletedTask;
        }
    }

    private static CallbackOidcContextBuilderResult External(
        string sub = "sub-123",
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

        var dispatcher = new FakeDispatcher();
        var resolver = new IdentityResolver(reader, dispatcher);

        var result = await resolver.ResolveAsync(External(), CancellationToken.None);

        result.Should().Be(existingId);
        reader.ReceivedExternalId.Should().Be("sub-123");

        // Should NOT create a new user
        dispatcher.ReceivedCommand.Should().BeNull();
    }

    [Fact]
    public async Task ResolveAsync_WhenUserDoesNotExist_CreatesUser()
    {
        var reader = new FakeReader { Returned = null };
        var dispatcher = new FakeDispatcher();

        var resolver = new IdentityResolver(reader, dispatcher);

        var result = await resolver.ResolveAsync(External(), CancellationToken.None);

        result.Should().Be(dispatcher.ReturnedId);

        dispatcher.ReceivedCommand.Should().BeOfType<CreateUserCommand>();

        var cmd = (CreateUserCommand)dispatcher.ReceivedCommand!;
        cmd.ExternalId.Should().Be("sub-123");
        cmd.FirstName.Should().Be("John");
        cmd.LastName.Should().Be("Doe");
        cmd.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task ResolveAsync_Throws_WhenCancellationRequested()
    {
        var reader = new FakeReader();
        var dispatcher = new FakeDispatcher();
        var resolver = new IdentityResolver(reader, dispatcher);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await resolver.ResolveAsync(External(), cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
