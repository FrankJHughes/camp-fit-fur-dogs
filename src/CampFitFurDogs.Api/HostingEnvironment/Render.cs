namespace CampFitFurDogs.Api.HostingEnvironment;

public static class Render
{
    /// <summary>
    /// Extracts the PR number from RENDER_EXTERNAL_URL.
    /// Expected format: https://pr-123-xxxxx.onrender.com
    /// </summary>
    public static bool TryGetPrNumber(string renderExternalUrl, string renderServiceName, out string? prNumber)
    {
        prNumber = null;
        // "https://campfitfurdogsapi-pr-209.onrender.com"

        var parts = renderExternalUrl.Split(
            [
                $"https://{renderServiceName}",
                "-pr-",
                ".onrender.com"
            ],
            StringSplitOptions.RemoveEmptyEntries);
        // [ "209" ]

        if (parts.Length < 1)
        {
            return false;
        }

        prNumber = parts[0];
        return true;
    }
}
