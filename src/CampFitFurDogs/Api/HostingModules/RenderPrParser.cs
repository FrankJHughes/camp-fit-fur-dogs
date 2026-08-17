namespace CampFitFurDogs.Api.HostingModules;

/// <summary>
/// Provides logic for parsing Render service names to extract pull‑request
/// numbers used in Render PR Preview environments.
/// <para>
/// Render assigns service names that typically follow a hyphen‑delimited pattern
/// (e.g., <c>campfitfurdogs-api-pr-123</c>).
/// This parser attempts to extract the trailing PR number so hosting modules can
/// adapt configuration based on the originating pull request.
/// </para>
/// </summary>
public sealed class RenderPrParser : IRenderPrParser
{
    /// <summary>
    /// Attempts to parse a Render service name and extract the associated
    /// pull‑request number, if present.
    /// </summary>
    /// <param name="renderServiceName">
    /// The Render service name to inspect. This value is typically provided via
    /// environment variables in Render PR Preview deployments.
    /// </param>
    /// <param name="prNumber">
    /// When the method returns <c>true</c>, contains the extracted pull‑request
    /// number. Otherwise, <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if a PR number could be successfully parsed; otherwise,
    /// <c>false</c>.
    /// </returns>
    public bool TryParse(string? renderServiceName, out string? prNumber)
    {
        prNumber = null;

        if (string.IsNullOrWhiteSpace(renderServiceName))
            return false;

        var parts = renderServiceName.Split(
            ['-'], StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
            return false;

        prNumber = parts.Last();
        return true;
    }
}
