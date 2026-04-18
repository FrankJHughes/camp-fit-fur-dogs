using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using CampFitFurDogs.Application;
using CampFitFurDogs.Application.Abstractions;
using TestDoubles.Dispatchers;

namespace CampFitFurDogs.Application.Tests.Dispatchers;

public class CommandDispatcherTests
{
    private static IServiceProvider BuildServices(
        bool includeHandler = true,
        bool includeInvalidValidator = false)
    {
        var services = new ServiceCollection();

        // Register ONLY the test handler
        if (includeHandler)
        {
            services.AddSingleton<TestCommandHandler>();
            services.AddSingleton<ICommandHandler<TestCommand, string>>(sp => sp.GetRequiredService<TestCommandHandler>());
        }

        // Optional validator
        if (includeInvalidValidator)
        {
            services.AddTransient<IValidator<TestCommand>, AlwaysInvalidCommandValidator>();
        }

        // Register dispatcher
        services.AddTransient<ICommandDispatcher, CommandDispatcher>();

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Dispatch_NoValidators_HandlerExecutes()
    {
        var provider = BuildServices(includeHandler: true);
        var dispatcher = provider.GetRequiredService<ICommandDispatcher>();

        var command = new TestCommand("hello");
        var result = await dispatcher.DispatchAsync(command, CancellationToken.None);

        result.Should().Be("OK");

        var handler = provider.GetRequiredService<TestCommandHandler>();
        handler.WasExecuted.Should().BeTrue();
        handler.Received.Should().Be(command);
    }

    [Fact]
    public async Task Dispatch_InvalidCommand_ThrowsValidationException()
    {
        var provider = BuildServices(includeHandler: true, includeInvalidValidator: true);
        var dispatcher = provider.GetRequiredService<ICommandDispatcher>();

        var command = new TestCommand("bad");
        var act = () => dispatcher.DispatchAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Dispatch_MissingHandler_Throws()
    {
        var provider = BuildServices(includeHandler: false);
        var dispatcher = provider.GetRequiredService<ICommandDispatcher>();

        var command = new TestCommand("hello");
        var act = () => dispatcher.DispatchAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
