using Frank.Core.Application.Abstractions.Clock;
using Frank.Core.Application.Abstractions.Observations;
using Frank.Core.Infrastructure.Clock;
using Frank.Core.Infrastructure.Observations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Frank.Core.Infrastructure.Tests.Observations.Unit;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddFrankCoreInfrastructureObservations_RegistersAllServices()
    {
        var services = new ServiceCollection();

        // REQUIRED dependencies for your DI extension
        services.AddHttpContextAccessor();
        services.AddSingleton<IHostEnvironment>(new FakeHostEnvironment());
        services.AddSingleton<IClock, SystemClock>();

        services.AddFrankCoreInfrastructureObservations();

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<IObservationSink>());
        Assert.NotNull(provider.GetService<IMetrics>());
        Assert.NotNull(provider.GetService<ICorrelationContext>());
        Assert.NotNull(provider.GetService<IErrorBoundaryObserver>());
        Assert.NotNull(provider.GetService<IRequestObservationContext>());
        Assert.NotNull(provider.GetService<Func<string, string, IObservationContext>>());
    }

    private sealed class FakeHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "TestApp";
        public string ContentRootPath { get; set; } = "";
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; }
            = new Microsoft.Extensions.FileProviders.NullFileProvider();
    }
}
