using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using SharedKernel.Api.Hosting;
using Xunit;

namespace SharedKernel.Api.Tests.Hosting;

public sealed class HostingProviderExtensionsTests
{
    // ------------------------------------------------------------
    // Fake provider for testing
    // ------------------------------------------------------------
    private sealed class FakeProvider : IHostingProvider
    {
        public string ProviderName { get; init; } = "Fake";
        public bool Active { get; init; }
        public bool ConfigureCalled { get; private set; }
        public Exception? ExceptionToThrow { get; init; }

        public bool IsActive() => Active;

        public Task ConfigureAsync(WebApplicationBuilder builder)
        {
            ConfigureCalled = true;

            if (ExceptionToThrow is not null)
                throw ExceptionToThrow;

            return Task.CompletedTask;
        }
    }

    // ------------------------------------------------------------
    // TESTS
    // ------------------------------------------------------------

    [Fact]
    public async Task First_active_provider_runs_and_stops_pipeline()
    {
        var builder = WebApplication.CreateBuilder();

        var p1 = new FakeProvider { ProviderName = "P1", Active = false };
        var p2 = new FakeProvider { ProviderName = "P2", Active = true };
        var p3 = new FakeProvider { ProviderName = "P3", Active = true };

        await builder.UseHostingProviders(p1, p2, p3);

        p1.ConfigureCalled.Should().BeFalse();
        p2.ConfigureCalled.Should().BeTrue();
        p3.ConfigureCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Inactive_providers_are_skipped()
    {
        var builder = WebApplication.CreateBuilder();

        var p1 = new FakeProvider { Active = false };
        var p2 = new FakeProvider { Active = false };

        await builder.UseHostingProviders(p1, p2);

        p1.ConfigureCalled.Should().BeFalse();
        p2.ConfigureCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Active_provider_exception_aborts_startup_outer_message()
    {
        var builder = WebApplication.CreateBuilder();

        var p1 = new FakeProvider
        {
            ProviderName = "Fake",
            Active = true,
            ExceptionToThrow = new InvalidOperationException("boom")
        };

        Func<Task> act = () => builder.UseHostingProviders(p1);

        var ex = await act.Should().ThrowAsync<InvalidOperationException>();

        ex.Which.Message.Should().Contain(
            "Hosting provider 'Fake' failed to configure the application");
        ex.Which.Message.Should().Contain("Startup aborted to protect production integrity");
    }

    [Fact]
    public async Task Active_provider_exception_aborts_startup_inner_message()
    {
        var builder = WebApplication.CreateBuilder();

        var p1 = new FakeProvider
        {
            ProviderName = "Fake",
            Active = true,
            ExceptionToThrow = new InvalidOperationException("boom")
        };

        Func<Task> act = () => builder.UseHostingProviders(p1);

        var ex = await act.Should().ThrowAsync<InvalidOperationException>();

        ex.Which.InnerException.Should().NotBeNull();
        ex.Which.InnerException!.Message.Should().Contain("boom");
    }

    [Fact]
    public async Task No_active_provider_does_not_throw()
    {
        var builder = WebApplication.CreateBuilder();

        var p1 = new FakeProvider { Active = false };
        var p2 = new FakeProvider { Active = false };

        await builder.UseHostingProviders(p1, p2);

        p1.ConfigureCalled.Should().BeFalse();
        p2.ConfigureCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Multiple_providers_only_first_active_runs()
    {
        var builder = WebApplication.CreateBuilder();

        var p1 = new FakeProvider { ProviderName = "P1", Active = false };
        var p2 = new FakeProvider { ProviderName = "P2", Active = true };
        var p3 = new FakeProvider { ProviderName = "P3", Active = true };
        var p4 = new FakeProvider { ProviderName = "P4", Active = true };

        await builder.UseHostingProviders(p1, p2, p3, p4);

        p1.ConfigureCalled.Should().BeFalse();
        p2.ConfigureCalled.Should().BeTrue();
        p3.ConfigureCalled.Should().BeFalse();
        p4.ConfigureCalled.Should().BeFalse();
    }
}
