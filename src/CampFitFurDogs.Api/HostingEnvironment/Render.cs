namespace CampFitFurDogs.Api.HostingEnvironment;

public static class Render
{
    /// <summary>
    /// Extracts the PR number from RENDER_EXTERNAL_URL.
    /// Expected format: https://pr-123-xxxxx.onrender.com
    /// </summary>
    public static string? GetPrNumber()
    {
        var url = Environment.GetEnvironmentVariable("RENDER_EXTERNAL_URL");
        if (string.IsNullOrWhiteSpace(url))
            return null;

        try
        {
            var host = new Uri(url).Host; // pr-123-abc123.onrender.com
            var firstPart = host.Split('.')[0]; // pr-123-abc123
            var parts = firstPart.Split('-');   // ["pr", "123", "abc123"]

            if (parts.Length < 2)
                return null;

            return parts[1]; // "123"
        }
        catch
        {
            return null;
        }
    }
}
