using CampFitFurDogs.Api.Horizontals.Cors;
using CampFitFurDogs.TestUtilities.Fakes;
using FluentAssertions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CampFitFurDogs.Api.Tests.Middleware;

public class CorsLoggingMiddlewareTests
{
    [Fact]
    public async Task Logs_allowed_origin_request()
    {
        var services = new ServiceCollection();
        var logger = new TestLogger<CorsLoggingMiddleware>();

        var policy = new CorsPolicyBuilder()
            .WithOrigins("https://allowed.com")
            .Build();

        services.AddSingleton<ICorsPolicyProvider>(new FakeCorsPolicyProvider(policy));
        services.AddSingleton<ILogger<CorsLoggingMiddleware>>(logger);

        var provider = services.BuildServiceProvider();

        var middleware = new CorsLoggingMiddleware(
            _ => Task.CompletedTask,
            provider.GetRequiredService<ILogger<CorsLoggingMiddleware>>(),
            provider.GetRequiredService<ICorsPolicyProvider>());

        var ctx = new DefaultHttpContext();
        ctx.Request.Headers["Origin"] = "https://allowed.com";
        ctx.Request.Method = "GET";
        ctx.Request.Path = "/api/test";

        await middleware.InvokeAsync(ctx);

        logger.Messages.Should().Contain(m => m.Contains("CORS request allowed"));
    }

    [Fact]
    public async Task Logs_blocked_origin_request()
    {
        var services = new ServiceCollection();
        var logger = new TestLogger<CorsLoggingMiddleware>();

        var policy = new CorsPolicyBuilder()
            .WithOrigins("https://allowed.com")
            .Build();

        services.AddSingleton<ICorsPolicyProvider>(new FakeCorsPolicyProvider(policy));
        services.AddSingleton<ILogger<CorsLoggingMiddleware>>(logger);

        var provider = services.BuildServiceProvider();

        var middleware = new CorsLoggingMiddleware(
            _ => Task.CompletedTask,
            provider.GetRequiredService<ILogger<CorsLoggingMiddleware>>(),
            provider.GetRequiredService<ICorsPolicyProvider>());

        var ctx = new DefaultHttpContext();
        ctx.Request.Headers["Origin"] = "https://evil.com";
        ctx.Request.Method = "POST";
        ctx.Request.Path = "/api/orders";

        await middleware.InvokeAsync(ctx);

        logger.Messages.Should().Contain(m => m.Contains("CORS request blocked"));
    }

    [Fact]
    public async Task Logs_allowed_preflight()
    {
        var services = new ServiceCollection();
        var logger = new TestLogger<CorsLoggingMiddleware>();

        var policy = new CorsPolicyBuilder()
            .WithOrigins("https://allowed.com")
            .Build();

        services.AddSingleton<ICorsPolicyProvider>(new FakeCorsPolicyProvider(policy));
        services.AddSingleton<ILogger<CorsLoggingMiddleware>>(logger);

        var provider = services.BuildServiceProvider();

        var middleware = new CorsLoggingMiddleware(
            _ => Task.CompletedTask,
            provider.GetRequiredService<ILogger<CorsLoggingMiddleware>>(),
            provider.GetRequiredService<ICorsPolicyProvider>());

        var ctx = new DefaultHttpContext();
        ctx.Request.Method = "OPTIONS";
        ctx.Request.Headers["Origin"] = "https://allowed.com";
        ctx.Request.Headers["Access-Control-Request-Method"] = "POST";

        await middleware.InvokeAsync(ctx);

        logger.Messages.Should().Contain(m => m.Contains("CORS preflight allowed"));
    }

    [Fact]
    public async Task Logs_blocked_preflight()
    {
        var services = new ServiceCollection();
        var logger = new TestLogger<CorsLoggingMiddleware>();

        var policy = new CorsPolicyBuilder()
            .WithOrigins("https://allowed.com")
            .Build();

        services.AddSingleton<ICorsPolicyProvider>(new FakeCorsPolicyProvider(policy));
        services.AddSingleton<ILogger<CorsLoggingMiddleware>>(logger);

        var provider = services.BuildServiceProvider();

        var middleware = new CorsLoggingMiddleware(
            _ => Task.CompletedTask,
            provider.GetRequiredService<ILogger<CorsLoggingMiddleware>>(),
            provider.GetRequiredService<ICorsPolicyProvider>());

        var ctx = new DefaultHttpContext();
        ctx.Request.Method = "OPTIONS";
        ctx.Request.Headers["Origin"] = "https://evil.com";
        ctx.Request.Headers["Access-Control-Request-Method"] = "POST";

        await middleware.InvokeAsync(ctx);

        logger.Messages.Should().Contain(m => m.Contains("CORS preflight blocked"));
    }

    [Fact]
    public async Task Logs_nothing_when_no_origin_header()
    {
        var services = new ServiceCollection();
        var logger = new TestLogger<CorsLoggingMiddleware>();

        var policy = new CorsPolicyBuilder()
            .WithOrigins("https://allowed.com")
            .Build();

        services.AddSingleton<ICorsPolicyProvider>(new FakeCorsPolicyProvider(policy));
        services.AddSingleton<ILogger<CorsLoggingMiddleware>>(logger);

        var provider = services.BuildServiceProvider();

        var middleware = new CorsLoggingMiddleware(
            _ => Task.CompletedTask,
            provider.GetRequiredService<ILogger<CorsLoggingMiddleware>>(),
            provider.GetRequiredService<ICorsPolicyProvider>());

        var ctx = new DefaultHttpContext();
        ctx.Request.Method = "GET";

        await middleware.InvokeAsync(ctx);

        logger.Messages.Should().BeEmpty();
    }
}
