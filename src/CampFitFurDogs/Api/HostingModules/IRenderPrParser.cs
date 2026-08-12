namespace CampFitFurDogs.Api.HostingModules;

/// <summary>
/// Defines an abstraction for parsing Render service names to extract
/// pull‑request numbers from Render PR Preview environments.
/// <para>
/// Render assigns service names that embed metadata such as the originating
/// pull‑request number. Implementations of this interface attempt to extract
/// that PR number so hosting modules can adapt configuration accordingly.
/// </para>
/// </summary>
public interface IRenderPrParser
{
    /// <summary>
    /// Attempts to parse a Render service name and extract the associated
    /// pull‑request number, if present.
    /// </summary>
    /// <param name="renderServiceName">
    /// The Render service name to inspect. This is typically provided via
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
    bool TryParse(string renderServiceName, out string? prNumber);
}
