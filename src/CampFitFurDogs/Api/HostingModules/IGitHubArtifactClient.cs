namespace CampFitFurDogs.Api.PlatformModules;

public interface IGitHubArtifactClient
{
    Task<string?> GetArtifactFileAsync(
        string githubToken,
        string repoSlug,
        string artifactName,
        string fileName);
}
