using Microsoft.Extensions.DependencyInjection;
using SharedKernel;
using SharedKernel.Abstractions;
using CampFitFurDogs.Application.Abstractions.Customers.FindCustomerByExternalId;
using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;
using CampFitFurDogs.Application.Abstractions.Identity.External;
using CampFitFurDogs.Infrastructure.Tests.Fakes;
using CampFitFurDogs.Infrastructure.Identity.Auth0;
using CampFitFurDogs.Domain.Customers.Exceptions;

namespace CampFitFurDogs.Application.Tests.Identity;

public class Auth0IdentityResolverTests
{
    private static IExternalIdentityResolver BuildResolver(
        FakeFindCustomerByExternalIdReader? reader = null,
        FakeCreateCustomerHandler? handler = null,
        ICommandDispatcher? dispatcher = null)
    {
        var services = new ServiceCollection();

        services.AddSingleton<IFindCustomerByExternalIdReader>(reader ?? new FakeFindCustomerByExternalIdReader());
        services.AddSingleton<ICommandHandler<CreateCustomerCommand, Guid>>(handler ?? new FakeCreateCustomerHandler());

        if (dispatcher is null)
        {
            services.AddSingleton<ICommandDispatcher>(sp => new CommandDispatcher(sp));
        }
        else
        {
            services.AddSingleton<ICommandDispatcher>(dispatcher);
        }

        services.AddSingleton<IExternalIdentityResolver, Auth0IdentityResolver>();

        return services.BuildServiceProvider().GetRequiredService<IExternalIdentityResolver>();
    }

    private static Task<Guid> Call(IExternalIdentityResolver resolver, string externalId)
        => resolver.ResolveAsync(
            externalId,
            "Frank",
            "Smith",
            "frank@example.com",
            CancellationToken.None);

    // ------------------------------------------------------------
    // 1. Existing customer path
    // ------------------------------------------------------------
    [Fact]
    public async Task Returns_existing_customer_when_found()
    {
        var existingId = Guid.NewGuid();

        var reader = new FakeFindCustomerByExternalIdReader
        {
            Response = new FindCustomerByExternalIdResponse(existingId)
        };

        var handler = new FakeCreateCustomerHandler();

        var resolver = BuildResolver(reader, handler);

        var result = await Call(resolver, "auth0|abc123");

        Assert.Equal(existingId, result);
        Assert.Null(handler.LastCommand);
    }

    // ------------------------------------------------------------
    // 2. New customer creation path
    // ------------------------------------------------------------
    [Fact]
    public async Task Creates_new_customer_when_not_found()
    {
        var newId = Guid.NewGuid();

        var reader = new FakeFindCustomerByExternalIdReader { Response = null };
        var handler = new FakeCreateCustomerHandler { ResultToReturn = newId };

        var resolver = BuildResolver(reader, handler);

        var result = await Call(resolver, "auth0|newuser");

        Assert.Equal(newId, result);
        Assert.NotNull(handler.LastCommand);
        Assert.Equal("Frank", handler.LastCommand!.FirstName);
        Assert.Equal("Smith", handler.LastCommand!.LastName);
        Assert.Equal("frank@example.com", handler.LastCommand!.Email);
        Assert.Equal("auth0|newuser", handler.LastCommand!.ExternalAuthProviderId);
    }

    // ------------------------------------------------------------
    // 3. Reader throws
    // ------------------------------------------------------------
    [Fact]
    public async Task Throws_when_reader_fails()
    {
        var reader = new FakeFindCustomerByExternalIdReader
        {
            ExceptionToThrow = new InvalidOperationException("reader failed")
        };

        var resolver = BuildResolver(reader);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Call(resolver, "auth0|abc123"));
    }

    // ------------------------------------------------------------
    // 4. Handler throws
    // ------------------------------------------------------------
    [Fact]
    public async Task Throws_when_handler_fails()
    {
        var reader = new FakeFindCustomerByExternalIdReader { Response = null };
        var handler = new FakeCreateCustomerHandler
        {
            ExceptionToThrow = new InvalidOperationException("handler failed")
        };

        var resolver = BuildResolver(reader, handler);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Call(resolver, "auth0|abc123"));
    }

    // ------------------------------------------------------------
    // 5. Dispatcher throws
    // ------------------------------------------------------------
    private sealed class ThrowingDispatcher : ICommandDispatcher
    {
        public Task<TResponse> DispatchAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct)
            => throw new InvalidOperationException("dispatcher failed");

        public Task DispatchAsync(ICommand command, CancellationToken ct)
            => throw new NotImplementedException();
    }

    [Fact]
    public async Task Throws_when_dispatcher_fails()
    {
        var reader = new FakeFindCustomerByExternalIdReader { Response = null };
        var handler = new FakeCreateCustomerHandler { ResultToReturn = Guid.NewGuid() };

        var resolver = BuildResolver(reader, handler, new ThrowingDispatcher());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Call(resolver, "auth0|abc123"));
    }

    // ------------------------------------------------------------
    // 7. Invalid externalId
    // ------------------------------------------------------------
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Throws_when_external_id_is_invalid(string? externalId)
    {
        var resolver = BuildResolver();

        await Assert.ThrowsAsync<MissingIdentitySourceException>(() =>
            resolver.ResolveAsync(
                externalId!,
                "Frank",
                "Smith",
                "frank@example.com",
                CancellationToken.None));
    }
}
