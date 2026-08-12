#nullable enable

using Frank.Core.Application.Abstractions.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Frank.Testing.Endpoints;

/// <summary>
/// A test‑only endpoint that intentionally throws an exception when invoked.
/// <para>
/// This endpoint is used to validate error‑handling behavior, exception
/// middleware, logging pipelines, and test client resilience.
/// It is not intended for production use.
/// </para>
/// </summary>
public sealed class ThrowEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the <c>/__test__/throw</c> endpoint, which throws an
    /// <see cref="InvalidOperationException"/> every time it is called.
    /// <para>
    /// The endpoint is marked <c>AllowAnonymous()</c> so tests can trigger
    /// exception paths without requiring authentication.
    /// </para>
    /// </summary>
    /// <param name="api">The API route group created by Frank.Core.</param>
    public void Map(RouteGroupBuilder api)
    {
        api.MapGet("/__test__/throw", () =>
        {
            throw new InvalidOperationException("Test exception");
        })
        .AllowAnonymous();
    }
}
