#nullable enable
using Microsoft.AspNetCore.Builder;

namespace Frank.Core.Api.Middleware.Exceptions;

/// <summary>
/// Provides extension methods for registering the
/// <see cref="ExceptionHandlingMiddleware"/> in the ASP.NET Core request pipeline.
/// <para>
/// This middleware centralizes exception handling for the API by unwrapping
/// common wrapper exceptions, resolving the correct exception handler from the
/// <see cref="Frank.Core.Infrastructure.Exceptions.ExceptionHandlerRegistry"/>,
/// and emitting structured <c>ProblemDetails</c> responses.
/// </para>
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="ExceptionHandlingMiddleware"/> to the application's
    /// middleware pipeline.
    /// <para>
    /// This should be placed early—typically immediately after routing and before
    /// endpoint execution—to ensure that unhandled exceptions from any vertical
    /// slice are captured and transformed into consistent <c>ProblemDetails</c>
    /// responses.
    /// </para>
    /// </summary>
    /// <param name="app">
    /// The <see cref="IApplicationBuilder"/> used to configure the request pipeline.
    /// </param>
    /// <returns>
    /// The same <see cref="IApplicationBuilder"/> instance, enabling fluent
    /// configuration.
    /// </returns>
    public static IApplicationBuilder UseFrankCoreApiMiddlewareExceptions(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
