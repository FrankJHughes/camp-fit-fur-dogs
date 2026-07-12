namespace CampFitFurDogs.Api.HostingModules;

public interface IGitHubArtifactClient
{
    Task<string?> GetArtifactFileAsync(
        string githubToken,
        string repoSlug,
        string artifactName,
        string fileName);
}
