using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;
using CampFitFurDogs.Infrastructure.Identity;
using Frank.Abstractions.Authentication.Callback;
using FluentAssertions;
using Frank.Abstractions.Command;
using CampFitFurDogs.Application.Abstractions.Customers.FindCustomerByExternalId;

namespace CampFitFurDogs.Infrastructure.Tests.Identity;

public sealed class IdentityResolverTests
{
    // ------------------------------------------------------------
    // FAKES
    // ------------------------------------------------------------

    private sealed class FakeReader : IFindCustomerByExternalIdReader
    {
        public string? ReceivedExternalId { get; private set; }
        public FindCustomerByExternalIdResponse? Returned { get; set; }

        public Task<FindCustomerByExternalIdResponse?> FindByExternalIdAsync(
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

    private static FrankAuthCallbackResult External(
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
    public async Task ResolveAsync_WhenCustomerExists_ReturnsExistingId()
    {
        var existingId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var reader = new FakeReader
        {
            Returned = new FindCustomerByExternalIdResponse(existingId)
        };

        var dispatcher = new FakeDispatcher();
        var resolver = new IdentityResolver(reader, dispatcher);

        var result = await resolver.ResolveAsync(External(), CancellationToken.None);

        result.Should().Be(existingId);
        reader.ReceivedExternalId.Should().Be("sub-123");

        // Should NOT create a new customer
        dispatcher.ReceivedCommand.Should().BeNull();
    }

    [Fact]
    public async Task ResolveAsync_WhenCustomerDoesNotExist_CreatesCustomer()
    {
        var reader = new FakeReader { Returned = null };
        var dispatcher = new FakeDispatcher();

        var resolver = new IdentityResolver(reader, dispatcher);

        var result = await resolver.ResolveAsync(External(), CancellationToken.None);

        result.Should().Be(dispatcher.ReturnedId);

        dispatcher.ReceivedCommand.Should().BeOfType<CreateCustomerCommand>();

        var cmd = (CreateCustomerCommand)dispatcher.ReceivedCommand!;
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
