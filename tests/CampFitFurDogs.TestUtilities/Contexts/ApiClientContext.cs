namespace CampFitFurDogs.TestUtilities.Contexts;

public sealed record ApiClientContext(
    string? AuthenticatedUserSub = null,
    Dictionary<string, string>? DefaultHeaders = null
)
{
    public ApiClientContext() : this(
        AuthenticatedUserSub: null,
        DefaultHeaders: new Dictionary<string, string>()
    )
    { }

    // -------------------------------------------------------
    // AUTHENTICATED USER
    // -------------------------------------------------------
    public ApiClientContext WithAuthenticatedUser(string? externalSub)
        => this with { AuthenticatedUserSub = externalSub };

    // -------------------------------------------------------
    // HEADERS
    // -------------------------------------------------------
    public ApiClientContext WithHeader(string key, string value)
    {
        var copy = new Dictionary<string, string>(DefaultHeaders!)
        {
            [key] = value
        };
        return this with { DefaultHeaders = copy };
    }
}
