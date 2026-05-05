namespace CampFitFurDogs.Api.HostingEnvironment;

public static class Render
{
    /// <summary>
    /// Extracts the PR number from RENDER_EXTERNAL_URL.
    /// Expected format: https://pr-123-xxxxx.onrender.com
    /// </summary>
    public static bool TryGetPrNumber(string renderServiceName, out string? prNumber)
    {
        prNumber = null;

        // "campfitfurdogsapi-pr-209"

        var parts = renderServiceName.Split(["-"], StringSplitOptions.RemoveEmptyEntries);
        // [ "campfitfurdogsapi", "pr", "209" ]

        if (parts.Length < 3)
        {
            return false;
        }

        prNumber = parts.Last();
        return true;
    }
}
