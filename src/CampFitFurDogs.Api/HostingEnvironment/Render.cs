namespace CampFitFurDogs.Api.HostingEnvironment;

public static class Render
{
    /// <summary>
    /// Extracts the PR number from RENDER_EXTERNAL_URL.
    /// Expected format: https://pr-123-xxxxx.onrender.com
    /// </summary>
    public static bool TryGetPrNumber(string renderExternalUrl, out string? prNumber)
    {
        prNumber = null;
        try
        {
            var host = new Uri(renderExternalUrl).Host; // pr-123-abc123.onrender.com
            var firstPart = host.Split('.')[0]; // pr-123-abc123
            var parts = firstPart.Split('-');   // ["pr", "123", "abc123"]

            if (parts.Length < 2)
                return false;

            prNumber = parts[1]; // "123"
            return true;
        }
        catch
        {
            return false;
        }
    }
}
