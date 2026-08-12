using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Frank.Core.Api.Routing.OpenApi;

/// <summary>
/// Provides extension methods for applying OpenAPI metadata to a
/// <see cref="RouteGroupBuilder"/>.
/// <para>
/// This replaces the deprecated <c>WithOpenApi()</c> extension and uses modern
/// endpoint metadata attributes that are automatically consumed by OpenAPI
/// generators such as Swashbuckle and NSwag.
/// </para>
/// </summary>
public static class RouteGroupBuilderExtensions
{
    /// <summary>
    /// Applies OpenAPI metadata to the specified route group.
    /// <para>
    /// This includes tags and a human‑readable description that will appear in
    /// generated OpenAPI documentation.
    /// </para>
    /// </summary>
    /// <param name="group">
    /// The route group to configure.
    /// </param>
    /// <param name="tag">
    /// The OpenAPI tag to apply to all endpoints in the group.
    /// </param>
    /// <param name="description">
    /// A human‑readable description of the API group.
    /// </param>
    /// <returns>
    /// The same <see cref="RouteGroupBuilder"/> instance.
    /// </returns>
    public static RouteGroupBuilder AddOpenApiMetadata(
        this RouteGroupBuilder group,
        string tag,
        string description)
    {
        group.WithTags(tag);
        group.WithDescription(description);

        return group;
    }
}
