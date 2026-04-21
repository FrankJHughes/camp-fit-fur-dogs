using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using CampFitFurDogs.Application;
using SharedKernel;
using SharedKernel.Abstractions;
using TestDoubles.Dispatchers;

namespace CampFitFurDogs.Application.Tests.Dispatchers;

public class QueryDispatcherTests
{
    private static IServiceProvider BuildServices(
        bool includeHandler = true,
        bool includeInvalidValidator = false)
    {
        var services = new ServiceCollection();

        // Register ONLY the test handler
        if (includeHandler)
        {
            services.AddSingleton<TestQueryHandler>();
            services.AddSingleton<IQueryHandler<TestQuery, string>>(sp => sp.GetRequiredService<TestQueryHandler>());
        }

        if (includeInvalidValidator)
        {
            services.AddTransient<IValidator<TestQuery>, AlwaysInvalidQueryValidator>();
        }

        services.AddTransient<IQueryDispatcher, QueryDispatcher>();

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Dispatch_NoValidators_HandlerExecutes()
    {
        var provider = BuildServices(includeHandler: true);
        var dispatcher = provider.GetRequiredService<IQueryDispatcher>();

        var query = new TestQuery("hello");
        var result = await dispatcher.DispatchAsync(query, CancellationToken.None);

        result.Should().Be("OK");

        var handler = provider.GetRequiredService<TestQueryHandler>();
        handler.WasExecuted.Should().BeTrue();
        handler.Received.Should().Be(query);
    }

    [Fact]
    public async Task Dispatch_InvalidQuery_ThrowsValidationException()
    {
        var provider = BuildServices(includeHandler: true, includeInvalidValidator: true);
        var dispatcher = provider.GetRequiredService<IQueryDispatcher>();

        var query = new TestQuery("bad");
        var act = () => dispatcher.DispatchAsync(query, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Dispatch_MissingHandler_Throws()
    {
        var provider = BuildServices(includeHandler: false);
        var dispatcher = provider.GetRequiredService<IQueryDispatcher>();

        var query = new TestQuery("hello");
        var act = () => dispatcher.DispatchAsync(query, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
