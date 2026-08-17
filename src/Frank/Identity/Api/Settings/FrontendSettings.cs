namespace Frank.Identity.Api.Settings;

/// <summary>
/// Configuration settings describing how the Identity API should communicate
/// with the frontend application.
/// <para>
/// This settings object is typically bound from configuration (e.g., <c>appsettings.json</c>)
/// and provides the base URL used when generating redirect links, callback URLs,
/// or any frontend‑facing navigation originating from the Identity subsystem.
/// </para>
/// </summary>
/// <remarks>
/// Although defined in the Identity API assembly, this setting is not identity‑specific.
/// It exists here because the Identity platform needs a reliable, centralized way
/// to reference the frontend application when performing login flows, logout flows,
/// or other cross‑application navigation.
/// </remarks>
public class FrontendSettings
{
    /// <summary>
    /// The base URL of the frontend application.
    /// <para>
    /// This value should be an absolute URL (e.g., <c>https://app.example.com</c>)
    /// and is used when constructing redirect targets for identity flows.
    /// </para>
    /// </summary>
    public string BaseUrl { get; set; } = default!;
}
