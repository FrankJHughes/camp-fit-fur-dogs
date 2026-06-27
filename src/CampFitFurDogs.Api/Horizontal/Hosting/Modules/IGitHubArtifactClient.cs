namespace CampFitFurDogs.Api.Horizontal.Hosting.Modules;

public interface IGitHubArtifactClient
{
    Task<string?> GetArtifactFileAsync(
        string githubToken,
        string repoSlug,
        string artifactName,
        string fileName);
}
