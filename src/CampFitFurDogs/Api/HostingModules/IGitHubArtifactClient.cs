namespace CampFitFurDogs.Api.HostingModules;

/// <summary>
/// Defines an abstraction for retrieving files from GitHub Actions artifacts.
/// <para>
/// Implementations of this interface provide the ability to query GitHub’s REST API,
/// locate artifacts by name, download their ZIP archives, and extract specific files.
/// </para>
/// </summary>
public interface IGitHubArtifactClient
{
    /// <summary>
    /// Retrieves a specific file from the latest GitHub Actions artifact matching
    /// the provided artifact name.
    /// </summary>
    /// <param name="githubToken">
    /// A GitHub token with permission to access repository artifacts.
    /// </param>
    /// <param name="repoSlug">
    /// The repository slug in the format <c>owner/repo</c>.
    /// </param>
    /// <param name="artifactName">
    /// The name of the artifact to search for.
    /// </param>
    /// <param name="fileName">
    /// The file inside the artifact ZIP archive to extract.
    /// </param>
    /// <returns>
    /// The file contents as a trimmed string, or <c>null</c> if the artifact or file
    /// cannot be found.
    /// </returns>
    Task<string?> GetArtifactFileAsync(
        string githubToken,
        string repoSlug,
        string artifactName,
        string fileName);
}
